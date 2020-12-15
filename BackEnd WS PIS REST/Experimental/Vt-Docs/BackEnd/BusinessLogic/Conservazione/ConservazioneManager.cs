using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using DocsPaVO.Conservazione;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;
using DocsPaDB.Query_DocsPAWS;
using BusinessLogic.Conservazione;
using BusinessLogic.Documenti;
using System.IO;
using System.Web;
using System.Data;

namespace BusinessLogic.Conservazione
{
    public class ConservazioneManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ConservazioneManager));

        public struct StatoIstanza
        {
            public const string DA_INVIARE = "N";
            public const string INVIATA = "I";
            public const string RIFIUTATA = "R";
            public const string IN_LAVORAZIONE = "L";
            public const string FIRMATA = "F";
            public const string CONSERVATA = "V";
            public const string CHIUSA = "C";
            // Stato in cui si trova dopo la verifica leggibilità e prima della chiusura. Per il caricamento asincrono.
            public const string IN_TRANSIZIONE = "T";
            // Stato in cui si trova durante le verifiche automatiche.
            public const string IN_FASE_VERIFICA = "Q";
            // Stato transitorio in cui si trova un'istanza durante le verifiche asincrone  
            public const string IN_VERIFICA = "A";
            // Stato  in cui si trova un'istanza alla fine delle verifiche asincrone  
            public const string VERIFICATA = "B";
            // Stato transitorio in cui si trova un'istanza durante la conversione asincrona degli item e degli allegati  
            public const string IN_CONVERSIONE = "Y";
            // Stato in cui si trova un'istanza nel caso in cui si verifichi un errore durante la conversione asincrona 
            // degli item e degli allegati
            public const string ERRORE_CONVERSIONE = "Z";
        }

        #region MEV 1.5 F02_01 formati conservazione

        /// <summary>
        /// Avvia la validazione e verifica di un'istanza elimindando dati relativi a verifiche precedenti(conservazione mev 1.5)
        /// </summary>
        /// <param name="idConservazione">Id dell'istanza</param>
        /// <returns></returns>
        public int startCheckAndValidateIstanzaConservazione(string idConservazione)
        {
            InfoUtente infoUtente = this.getInfoUtenteFromIdConservazione(idConservazione);

            int result = (int)InfoConservazione.EsitoVerifica.NonEffettuata;

            try
            {
                //elimino i dati presenti nella tabella di report relativi all'istanza poichè potrebbe essere 
                //lanciata la verifica una seconda volta. Aggiorno lo stato dell'istanza
                if (this.deleteReportFormatiConservazione(idConservazione))

                    // Reperimento elementi di conservazione e verifica, controlli su di essi, popolamento della tabella di report 
                    // ed infine aggiornamento dello stato dell'istanza
                    result = this.CheckAndValidationItemsConservazione(idConservazione, infoUtente);
            }
            catch (Exception e)
            {
                //stato verificata esito verifica in errore
                this.updateStatoConservazioneWhithEsitoVerifica(idConservazione, StatoIstanza.VERIFICATA, (int)InfoConservazione.EsitoVerifica.Errore);
                result = (int)InfoConservazione.EsitoVerifica.Errore;
            }

            return result;
        }


        private bool deleteReportFormatiConservazione(string idConservazione)
        {
            return this.DeleteItemReportFormatiConservazioneByIdIstCons(idConservazione);
        }

        /// <summary>
        /// Verifica dei formati per l'istanza conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        public int CheckAndValidationItemsConservazione(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int esitoVerifica = (int)InfoConservazione.EsitoVerifica.NonEffettuata;
            string err = string.Empty;
            logger.Debug("CheckAndValidationItemsConservazione start");
            //risultati
            List<ItemsConservazione> listaItem;
            List<ReportFormatiConservazione> reportList = new List<ReportFormatiConservazione>();

            try
            {
                ReportFormatiConservazione rigaReportFromati;
                bool ammesso = false;
                bool valido = false;
                bool consolidato = false;
                bool marcato = false;
                bool firmato = false;
                string utente = string.Empty;
                string ruolo = string.Empty;
                bool convertibile = false;
                string userRights = string.Empty;
                string idDocPrincipale = string.Empty;
                bool daValidare = true;
                string extOriginale = string.Empty;
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();


                listaItem = this.getItemsConservazioneByIdCons(idConservazione, infoUtente);
                if (listaItem != null && listaItem.Count > 0)
                {
                    foreach (ItemsConservazione itemsCons in listaItem)
                    {

                        rigaReportFromati = new ReportFormatiConservazione();
                        rigaReportFromati.ID_Istanza = idConservazione;
                        rigaReportFromati.ID_Item = itemsCons.SystemID;
                        rigaReportFromati.DocNumber = itemsCons.DocNumber;
                        rigaReportFromati.TipoFile = "P";
                        rigaReportFromati.ID_Project = itemsCons.ID_Project;
                        rigaReportFromati.ID_DocPrincipale = idDocPrincipale = itemsCons.DocNumber;
                        rigaReportFromati.Estensione = itemsCons.tipoFile;
                        if (itemsCons != null && itemsCons.immagineAcquisita != null)
                        {
                            try
                            {
                                //version Id
                                rigaReportFromati.Version_ID = this.getLatestVersionID(rigaReportFromati.DocNumber, infoUtente);

                                //verifica marca
                                marcato = this.checkDocumentoMarcato(itemsCons, infoUtente, rigaReportFromati.Version_ID);
                                rigaReportFromati.Marcata = marcato ? "1" : "0";

                                //firma
                                firmato = this.checkDocumentoFirmato(itemsCons, rigaReportFromati.Version_ID, out extOriginale);
                                rigaReportFromati.Firmata = firmato ? "1" : "0";

                                //se l'estensione originale è diversa da null la considero al posto di quella dell'items
                                if (!string.IsNullOrEmpty(extOriginale) && !extOriginale.Equals("0"))
                                {
                                    itemsCons.immagineAcquisita = "." + extOriginale.Trim();
                                    rigaReportFromati.Estensione = "." + extOriginale.Trim();
                                }

                                //VERIFICo FORMATO e valido
                                ammesso = this.checkFormatItemConservazione(itemsCons, infoUtente, out convertibile, out daValidare);
                                rigaReportFromati.Ammesso = ammesso ? "1" : "0";

                                //Verifica Consolidamento
                                consolidato = this.checkItemConsolidato(itemsCons.DocNumber);
                                rigaReportFromati.Consolidato = consolidato ? "1" : "0";


                                //gestione validazione: se il formato è da validare viene validato altrimenti 
                                if (daValidare)
                                {
                                    valido = this.validateItemConservazione(itemsCons, infoUtente);
                                    rigaReportFromati.Valido = valido ? "1" : "0";

                                    if (ammesso && valido)
                                    {
                                        //non serve memorizzare il flag di convertibilità visto che è sia ammesso e valido
                                        rigaReportFromati.Convertibile = "2";
                                    }
                                    else
                                    {
                                        //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido e non sia conslidato
                                        rigaReportFromati.Convertibile = convertibile && valido && !consolidato && !marcato && !firmato ? "1" : "0";
                                    }

                                }
                                else //non è da validare il formato
                                {
                                    //lo considero valido per i controlli interni ma non scrivo 
                                    //il valore della validazione nella riga
                                    valido = false;
                                    rigaReportFromati.Valido = "2";

                                    //se è ammesso non è soggetto alla conversione
                                    if (ammesso)
                                    {
                                        rigaReportFromati.Convertibile = "2";
                                    }
                                    else
                                    {
                                        //altrimenti è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido e non sia conslidato
                                        rigaReportFromati.Convertibile = convertibile && !consolidato && !marcato && !firmato ? "1" : "0";
                                    }
                                }

                                //ruolo e utente proprietario
                                //doc.getUtenteAndRuoloProprietario(itemsCons.DocNumber, out utente, out ruolo);
                                this.getUtenteAndRuoloProprietario(itemsCons.DocNumber, out utente, out ruolo);

                                rigaReportFromati.UtProp = utente;
                                rigaReportFromati.RuoloProp = ruolo;

                                //diritto di scrittura dell'utente che ha creato l'istanza
                                userRights = this.tipoDirittoUtenteCons(itemsCons.DocNumber, infoUtente);

                                rigaReportFromati.Modifica = userRights;


                                //nessun errore
                                rigaReportFromati.Errore = "0";
                            }
                            catch (Exception e)
                            {
                                rigaReportFromati.Errore = "1";
                                rigaReportFromati.TipoErrore = "1";//e.Message;
                            }
                            reportList.Add(rigaReportFromati);

                            //ALLEGATI!!!!
                            //caricamento allegati
                            ArrayList allegati = doc.GetAllegati(itemsCons.DocNumber, string.Empty);
                            if (allegati != null && allegati.Capacity > 0)
                            {
                                for (int i = 0; i < allegati.Count; i++)
                                {
                                    DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];

                                    rigaReportFromati = new ReportFormatiConservazione();
                                    rigaReportFromati.ID_Istanza = idConservazione;
                                    rigaReportFromati.ID_Item = itemsCons.SystemID;
                                    rigaReportFromati.DocNumber = all.docNumber;
                                    rigaReportFromati.TipoFile = "A";
                                    rigaReportFromati.ID_Project = itemsCons.ID_Project;
                                    rigaReportFromati.Version_ID = all.versionId;
                                    rigaReportFromati.ID_DocPrincipale = idDocPrincipale;
                                    try
                                    {
                                        //verifica marca
                                        marcato = this.checkAllegatoMarcato(all, infoUtente, rigaReportFromati.Version_ID);
                                        rigaReportFromati.Marcata = marcato ? "1" : "0";

                                        //reperimento estensione originale
                                        extOriginale = this.getOriginalExtension(all.docNumber, all.versionId);
                                        if (!string.IsNullOrEmpty(extOriginale))
                                        {
                                            rigaReportFromati.Estensione = extOriginale;
                                        }
                                        else
                                        {
                                            rigaReportFromati.Estensione = all.fileName.Substring(all.fileName.LastIndexOf("."));
                                        }

                                        // Determina se il formato è valido per la conservazione
                                        ammesso = this.checkFormatAllegato(all, infoUtente, rigaReportFromati.Estensione, out convertibile, out daValidare);
                                        rigaReportFromati.Ammesso = ammesso ? "1" : "0";

                                        firmato = !string.IsNullOrEmpty(all.firmato) && all.firmato.Equals("1");
                                        rigaReportFromati.Firmata = firmato ? "1" : "0";

                                        //considero il flag di consolidamento del principale poichè
                                        //se il principale è consolidato lo è anche l'allegato
                                        rigaReportFromati.Consolidato = consolidato ? "1" : "0";



                                        if (daValidare)
                                        {
                                            valido = this.validateAllegato(all.docNumber, infoUtente);
                                            rigaReportFromati.Valido = valido ? "1" : "0";

                                            if (ammesso && valido)
                                            {
                                                //non serve memorizzare il flag di convertibilità visto che è sia ammesso e valido
                                                rigaReportFromati.Convertibile = "2";
                                            }
                                            else
                                            {
                                                //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido ed il documento prncipale 
                                                //non sia consolidato
                                                rigaReportFromati.Convertibile = convertibile && valido && !consolidato && !marcato && !firmato ? "1" : "0";

                                            }

                                        }
                                        else //non è da validare il formato
                                        {
                                            valido = false;
                                            rigaReportFromati.Valido = "2";
                                            //se è ammesso non è soggetto alla conversione
                                            if (ammesso)
                                            {
                                                rigaReportFromati.Convertibile = "2";
                                            }
                                            else
                                            {
                                                //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido e non sia conslidato
                                                rigaReportFromati.Convertibile = convertibile && !consolidato && !marcato && !firmato ? "1" : "0";
                                            }
                                        }
                                        //ruolo e utente proprietario sono quelli del documento principale
                                        //doc.getUtenteAndRuoloProprietario(all.docNumber, out utente, out ruolo);

                                        rigaReportFromati.UtProp = utente;
                                        rigaReportFromati.RuoloProp = ruolo;

                                        //diritto di scrittura dell'utente che ha creato l'istanza, è lo stesso del documento principale
                                        //userRights = doc.getAccessRightDocBySystemID(all.docNumber, infoUtente);
                                        rigaReportFromati.Modifica = userRights;

                                        //nessun errore
                                        rigaReportFromati.Errore = "0";
                                    }
                                    catch (Exception e)
                                    {
                                        rigaReportFromati.Errore = "1";
                                        rigaReportFromati.TipoErrore = "1";//e.Message;
                                    }
                                    reportList.Add(rigaReportFromati);


                                }
                            }
                        }
                    }

                    //salvo i dati nel db
                    bool esitoUpdate = this.InsertListItemReportFormatiConservazione(reportList);
                    if (esitoUpdate)
                    {
                        //aggiorno lo stato dell'istanza di conservazione, in verificato, e l'esito 
                        esitoVerifica = this.getEsitoVerificaIstanzaCons(reportList);
                        if (!this.updateStatoConservazioneWhithEsitoVerifica(idConservazione, StatoIstanza.VERIFICATA, esitoVerifica))
                        {
                            esitoVerifica = (int)InfoConservazione.EsitoVerifica.Errore;
                            logger.Debug(String.Format("Errore aggiornamento dello stato dell'istanza {0} allo stato {1} con esito di verifica pari a {2}", idConservazione, StatoIstanza.VERIFICATA, esitoVerifica.ToString()));
                        }
                    }
                    else
                    {   //errore inserimento righe nella dpa_verifica_formati_cons imposto lo stato dell'istanza a errore
                        this.updateStatoConservazioneWhithEsitoVerifica(idConservazione, StatoIstanza.VERIFICATA, (int)InfoConservazione.EsitoVerifica.Errore);
                        esitoVerifica = (int)InfoConservazione.EsitoVerifica.Errore;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Debug(String.Format("Errore durante la verifica dell'istanza {0} Errore {1}", idConservazione.ToString(), exc.Message));
                throw new Exception(String.Format("Errore durante la verifica dell'istanza {0} Errore {1}", idConservazione.ToString(), exc.Message));
            }
            return esitoVerifica;

        }


        private string tipoDirittoUtenteCons(string docNumber, InfoUtente infoUtente)
        {
            //nessun diritto
            string result = "0";

            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                string userRights = doc.getAccessRightDocBySystemID(docNumber, infoUtente);
                //diritto scrittura o proprietario
                if (userRights.Equals("63") || userRights.Equals("255"))
                    result = "2";
                else if (userRights.Equals("45")) //lettura
                    result = "1";

            }
            catch (Exception e)
            {
                result = "0";
            }

            return result;
        }

        private string getLatestVersionID(string DocNumber, InfoUtente infoUtente)
        {
            string result = string.Empty;
            try
            {
                result = VersioniManager.getLatestVersionID(DocNumber, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getLatestVersionID", e);
                throw new Exception("Errore verifica formati");
            }
            return result;
        }

        private bool checkDocumentoMarcato(ItemsConservazione itemsCons, InfoUtente infoUtente, string version_id)
        {
            bool result = false;
            int countTimestamp = 0;
            try
            {

                if (itemsCons.tipoFile.ToUpper().Contains("TSD") || itemsCons.tipoFile.ToUpper().Contains("M7M"))
                    result = true;

                countTimestamp = BusinessLogic.Documenti.TimestampManager.getCountTimestampsDocLite(infoUtente, itemsCons.DocNumber, version_id);
                if (countTimestamp > -1)
                    result = result || (countTimestamp > 0);
                else
                {
                    throw new Exception("Errore verifica formati");
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkDocumentoMarcato", e);
                throw new Exception("Errore verifica formati");
            }

            return result;
        }

        private bool checkAllegatoMarcato(DocsPaVO.documento.Allegato all, InfoUtente infoUtente, string version_id)
        {
            bool result = false;
            int countTimestamp = 0;
            try
            {
                string ext = all.fileName.Substring(all.fileName.LastIndexOf("."));
                if (ext.ToUpper().Contains("TSD") || ext.ToUpper().Contains("M7M"))
                    result = true;

                countTimestamp = BusinessLogic.Documenti.TimestampManager.getCountTimestampsDocLite(infoUtente, all.docNumber, version_id);
                if (countTimestamp > -1)
                    result = result || (countTimestamp > 0);
                else
                {
                    throw new Exception("Errore verifica formati");
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkDocumentoMarcato", e);
                throw new Exception("Errore verifica formati");
            }

            return result;
        }



        private bool checkDocumentoFirmato(ItemsConservazione itemsCons, string version_id, out string extOriginale)
        {
            bool result = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                result = doc.isDocFirmato(itemsCons.DocNumber, version_id, out extOriginale);


            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkDocumentoMarcato", e);
                throw new Exception("Errore verifica formati");
            }

            return result;
        }

        private string getOriginalExtension(string DocNumber, string version_id)
        {
            string result = string.Empty;

            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                result = doc.getOriginaExt(DocNumber, version_id);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getOriginalExtension", e);
                throw new Exception("Errore verifica formati");
            }

            return result;
        }

        private int getEsitoVerificaIstanzaCons(List<ReportFormatiConservazione> reportList)
        {
            int esito = 0;

            try
            {
                int numElementiInErrore = reportList.Where(x => x.Errore.Equals("1")).Count();
                if (numElementiInErrore > 0)
                {
                    esito = (int)InfoConservazione.EsitoVerifica.Errore;
                }
                else
                {
                    int numTotElementi = reportList.Count;
                    int numElementiValidi = reportList.Where(x => x.Ammesso.Equals("1") && (x.Valido.Equals("1") || x.Valido.Equals("2"))).Count();
                    if (numTotElementi == numElementiValidi)
                    {
                        esito = (int)InfoConservazione.EsitoVerifica.Successo;
                    }
                    else
                    {
                        int numElementiNonValidi = reportList.Where(x => x.Valido.Equals("0")).Count();
                        int numElementiNonConvertibili = reportList.Where(x => x.Ammesso.Equals("0") && x.Convertibile.Equals("0")).Count();
                        if (numElementiNonValidi > 0 || numElementiNonConvertibili > 0)
                        {
                            esito = (int)InfoConservazione.EsitoVerifica.Fallita;
                        }
                        else
                        {
                            int numElementiNonConvertDirettamente = reportList.Where(x => x.Ammesso.Equals("0") && x.Convertibile.Equals("1")
                                                                        && (x.Modifica.Equals("0") || x.Modifica.Equals("1"))).Count();
                            if (numElementiNonConvertDirettamente > 0)
                            {
                                esito = (int)InfoConservazione.EsitoVerifica.IndirettamenteConvertibili;
                            }
                            else
                            {
                                esito = (int)InfoConservazione.EsitoVerifica.DirettamenteConvertibili;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                esito = (int)InfoConservazione.EsitoVerifica.Errore;
            }

            return esito;

        }

        private bool validateItemConservazione(ItemsConservazione itemsCons, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = !this.verificaTipoFile(itemsCons, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: validateItemConservazione", e);
                throw new Exception("Errore validazione");
            }
            return result;
        }

        private bool checkFormatItemConservazione(ItemsConservazione itemsCons, InfoUtente infoUtente, out bool convertibile, out bool daValidare)
        {
            bool result = false;
            convertibile = false;
            daValidare = false;
            try
            {
                DocsPaVO.FormatiDocumento.SupportedFileType supp =
                                 BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), itemsCons.immagineAcquisita.Replace(".", string.Empty));

                if (supp != null)
                {
                    if (supp.FileTypeUsed && supp.FileTypePreservation)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        if (itemsCons.tipoFile.ToUpper().Contains("HTML"))
                            convertibile = true;
                    }

                    if (supp.FileTypeValidation)
                        daValidare = true;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkFormatItemConservazione", e);
                throw new Exception("Errore verifica formati");
            }
            return result;

        }

        private bool validateAllegato(string docNumber, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = !this.verificaTipoFileAll(docNumber, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: validateAllegato", e);
                throw new Exception("Errore validazione");
            }
            return result;
        }

        private bool checkFormatAllegato(DocsPaVO.documento.Allegato allegato, InfoUtente infoUtente, string extOriginale, out bool convertibile, out bool daValidare)
        {
            bool result = false;
            convertibile = true;
            daValidare = false;
            try
            {
                string extension = string.Empty;

                if (!string.IsNullOrEmpty(extOriginale) && !extOriginale.Equals("0"))
                {
                    extension = extOriginale;
                }
                else
                {
                    extension = allegato.fileName.Substring(allegato.fileName.LastIndexOf(".") + 1);
                }

                DocsPaVO.FormatiDocumento.SupportedFileType supp =
                                 BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), extension);

                if (supp != null)
                {
                    if (supp.FileTypeUsed && supp.FileTypePreservation)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        //convertibile = true;
                    }
                    if (supp.FileTypeValidation)
                        daValidare = true;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkFormatAllegato", e);
                throw new Exception("Errore verifica formato");
            }

            return result;
        }

        /// <summary>
        /// Gestione dell'istanza dopo la conversione di un documento in conversione
        /// </summary>
        public void handleConsAutomaticConversion(InfoUtente infoUser, DocsPaVO.documento.SchedaDocumento schDoc)
        {
            try
            {
                logger.Debug("BEGIN - handleConsAutomaticConversion in ConservazioneManager");
                if (schDoc != null && schDoc.documenti[0] != null)
                {
                    logger.Debug("schDoc.systemid = " + schDoc.systemId);
                    DocsPaVO.documento.FileRequest fr = ((DocsPaVO.documento.FileRequest)schDoc.documenti[0]);
                    if (fr != null)
                    {
                        logger.Debug("fr.docNumber = " + fr.docNumber);
                        //1 verifico se è un documento appartente ad un'istanza di consercazione in convertirsione automatica
                        //string lastVersion = this.getLatestVersionID(schDoc.docNumber, infoUser);
                        ReportFormatiConservazione document = this.getReportFormatiConservazioneByDocNumber(schDoc.docNumber);

                        logger.Debug("versionId = " + fr.versionId);
                        if (document != null && !document.Version_ID.Equals(fr.versionId))
                        {
                            logger.Debug("document.System_ID = " + document.System_ID);
                            //2 se lo è aggiorno la riga ReportFormatiConservazione associata al documento
                            //  e verifico se tutti i documenti dell'istanza sono stati convertiti
                            //3 se lo sono aggiorno lo stato dell'istanza di conservazione
                            //TUTTO TRAMITE UNA SP
                            this.checkReportFormatiConservazioneAndUpdateStatoIstanzaCons(document, fr.versionId);
                        }
                        else
                        {   //ALLEGATI
                            logger.Debug("CONTROLLO GLI ALLEGATI");

                            if (schDoc.allegati != null && schDoc.allegati.Capacity > 0)
                            {
                                List<DocsPaVO.documento.Allegato> listAllegato = schDoc.allegati.Cast<DocsPaVO.documento.Allegato>().ToList();
                                listAllegato = listAllegato.Where(x => x != null && x.docNumber != null).ToList();
                                logger.Debug("listAllegato.count = " + listAllegato.Count.ToString());

                                foreach (DocsPaVO.documento.Allegato all in listAllegato)
                                {
                                    fr = (DocsPaVO.documento.FileRequest)all;
                                    if (fr != null)
                                    {
                                        logger.Debug("allegato.Docnumber = " + all.docNumber);
                                        //lastVersion = this.getLatestVersionID(all.docNumber, infoUser);
                                        logger.Debug("versionId = " + fr.versionId);
                                        logger.Debug("fr.docNumber ALLEGATO = " + fr.docNumber + "versionId = " + fr.versionId);

                                        document = this.getReportFormatiConservazioneByDocNumber(all.docNumber);
                                        if (document != null && !document.Version_ID.Equals(fr.versionId))
                                        {
                                            logger.Debug("document.System_ID = " + document.System_ID);
                                            //2 se lo è aggiorno la riga ReportFormatiConservazione associata al documento
                                            //  e verifico se tutti i documenti dell'istanza sono stati convertiti
                                            //3 se lo sono aggiorno lo stato dell'istanza di conservazione
                                            //TUTTO TRAMITE UNA SP
                                            this.checkReportFormatiConservazioneAndUpdateStatoIstanzaCons(document, fr.versionId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                logger.Debug("END - handleConsAutomaticConversion in ConservazioneManager");
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: handleConsAutomaticConversion", e);
            }
        }

        /// <summary>
        /// Avvia la conversione dei documenti dell'istanza e aggiorna lo stato dell'istanza in base al risultato
        /// </summary
        public bool convertAndSendForConservation(string idConservazione, bool conSecurity, HttpContext context)
        {
            bool esito = false;
            InfoUtente infoUtente = this.getInfoUtenteFromIdConservazione(idConservazione);

            List<ReportFormatiConservazione> listaReport;
            List<ReportFormatiConservazione> docDaConvertire;

            SchedaDocumento schDoc = null;
            FileDocumento fileDocumento = null;

            ObjServerPdfConversion objServerPdfConversion;
            //int idAmm;
            //Int32.TryParse(infoUtente.idAmministrazione, out idAmm);
            try
            {
                //prendo i documenti associati all'istanza
                listaReport = this.getItemReportFormatiConservazioneByIdIstCons(idConservazione);
                if (listaReport != null && listaReport.Count > 0)
                {
                    if (conSecurity)
                    {
                        //prendo tutti i documenti da convertire considerando la security
                        docDaConvertire = listaReport.Where(x => x.Ammesso.Equals("0") && x.Modifica.Equals("2")
                                                                && !x.Valido.Equals("0") && x.Convertibile.Equals("1")).ToList();
                    }
                    else
                    {
                        //prendo tutti i documenti da convertire senza considerare la security
                        //per i casi di conversione da policy automatiche
                        docDaConvertire = listaReport.Where(x => x.Ammesso.Equals("0") && !x.Valido.Equals("0") && x.Convertibile.Equals("1")).ToList();
                    }
                    foreach (ReportFormatiConservazione doc in docDaConvertire)
                    {
                        try
                        {

                            //prendo la SchedaDocumento del documento (solo con il docNubmer e non curandmi della visibilità)
                            schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.DocNumber);
                            if (schDoc != null)
                            {
                                logger.Debug("schDoc != null");
                                fileDocumento = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schDoc.documenti[0], infoUtente);
                                if (fileDocumento != null)
                                {
                                    logger.Debug("fileDocumento != null");
                                    //creo per ogni documento l'oggetto objServerPdfConversion
                                    objServerPdfConversion = new ObjServerPdfConversion();

                                    objServerPdfConversion.docNumber = doc.DocNumber;
                                    objServerPdfConversion.idProfile = schDoc.systemId;
                                    objServerPdfConversion.fileName = ((DocsPaVO.documento.FileRequest)schDoc.documenti[0]).fileName;
                                    objServerPdfConversion.content = fileDocumento.content;
                                    logger.DebugFormat("docNumber: {0}, idProfile: {1}, filename: {2}, content.lenght: {3}", objServerPdfConversion.docNumber,
                                        objServerPdfConversion.idProfile, objServerPdfConversion.fileName, objServerPdfConversion.content.Count());
                                    //l'url della wa è null poichè la chiamata viene fatta dal backend, la imposto a stringa vuota
                                    if (string.IsNullOrEmpty(infoUtente.urlWA))
                                        infoUtente.urlWA = string.Empty;

                                    //e le aggiungo in coda di conversione PDF
                                    BusinessLogic.LiveCycle.LiveCyclePdfConverter.EnqueueServerPdfConversion(infoUtente, objServerPdfConversion, context);
                                    doc.DaConverire = "1";
                                    doc.Convertito = "0";
                                }
                                else
                                {
                                    //fileDocumento = null aggiorno errore conversione item e lo segno come convertito cosi poi 
                                    //il metodo, chiamato al dequeue del file pdf, che controlla lo considera come processato
                                    logger.Debug("ConservazioneManager  - metodo: convertAndSendForConservation, fileDocumento = null, DOCUMENTO " + doc.DocNumber);
                                    doc.DaConverire = "1";
                                    doc.Convertito = "1";
                                    doc.Errore = "1";
                                    doc.TipoErrore = "2";

                                }

                            }
                            else
                            {
                                //scheda doc = null aggiorno errore conversione item e lo segno come convertito cosi poi 
                                //il metodo, chiamato al dequeue del file pdf, che controlla lo considera come processato
                                logger.Debug("ConservazioneManager  - metodo: convertAndSendForConservation, scheda doc = null; DOCUMENTO " + doc.DocNumber);
                                doc.DaConverire = "1";
                                doc.Convertito = "1";
                                doc.Errore = "1";
                                doc.TipoErrore = "2";
                            }

                            esito = this.UpdateItemReportFormatiConservazione(doc);
                        }
                        catch (Exception exDoc)
                        {
                            //scheda doc = null aggiorno errore conversione item e lo segno come convertito cosi poi 
                            //il metodo, chiamato al dequeue del file pdf, che controlla lo considera come processato
                            logger.Debug("Errore in ConservazioneManager  - metodo: convertAndSendForConservation, DOCUMENTO " + doc.DocNumber, exDoc);
                            doc.DaConverire = "1";
                            doc.Convertito = "1";
                            doc.Errore = "1";
                            doc.TipoErrore = "2";
                            esito = false;

                            this.UpdateItemReportFormatiConservazione(doc);
                        }
                    }

                    //controllo se tutti i documenti associati all'istanza sono stati processati (se lo sono 
                    //significa che sono tutti in errore)

                    int docInErrore = listaReport.Where(x => x.DaConverire.Equals("1") &&
                                                        x.Convertito.Equals("1") && x.Errore.Equals("1")).Count();

                    if (docDaConvertire.Count == docInErrore)
                        this.updateStatoConservazione(idConservazione, StatoIstanza.ERRORE_CONVERSIONE);

                }
                else
                {
                    //errore non ci sono documenti associati all'istanza
                    //aggiorno lo stato dell'istanza

                    this.updateStatoConservazione(idConservazione, StatoIstanza.ERRORE_CONVERSIONE);
                }

            }
            catch (Exception e)
            {
                //stato errore conversione
                this.updateStatoConservazione(idConservazione, StatoIstanza.ERRORE_CONVERSIONE);
                logger.Debug("Errore in ConservazioneManager  - metodo: convertAndSendForConservation", e);
                esito = false;
            }
            return esito;
        }




        #endregion


        #region COPIA DocsPaConsManager dipendenze

        private InfoUtente getInfoUtenteFromIdConservazione(string idConservazione)
        {
            InfoUtente infoUtente = null;
            InfoConservazione infoCons = this.getInfoConsercazioneByid(idConservazione);

            if (infoCons != null)
            {
                infoUtente = this.getInfoUtenteConservazione(infoCons);
                if (infoUtente != null)
                {
                    DocsPaDocumentale.Documentale.UserManager um = new DocsPaDocumentale.Documentale.UserManager();
                    infoUtente.dst = um.GetSuperUserAuthenticationToken();
                }
            }
            return infoUtente;
        }

        public DocsPaVO.areaConservazione.InfoConservazione getInfoConsercazioneByid(string idConservazione)
        {
            InfoConservazione result;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.getInfoConsercazioneByidCons(idConservazione);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getItemsConservazioneByIdCons", e);
                result = null;
            }
            return result;
        }

        private bool verificaTipoFile(DocsPaVO.areaConservazione.ItemsConservazione itemsCons, InfoUtente infoUtente)
        {
            if (infoUtente == null)
                return false;

            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, itemsCons.DocNumber);
            DocsPaVO.documento.FileDocumento fd = null;

            if (sch.inCestino == null)
                sch.inCestino = string.Empty;


            if (sch.inCestino != "1")
            {
                try
                {
                    //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                    if (sch.documenti != null && sch.documenti[0] != null &&
                            Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];

                        fd = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                        if (fd == null)
                            throw new Exception("Errore nel reperimento del file principale.");


                        Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
                        string ext = ff.FileType(fd.content).ToLower();

                        if (ff.isExecutable(ext))
                        {
                            logger.DebugFormat("Validazione Allegato fallita, riscontrato codice eseguibile in {0}", fd.name);
                            return true;
                        }
                        // string estensione =itemsCons.immagineAcquisita.Replace(".", string.Empty).ToLower ();
                        string estensione = Path.GetExtension(fd.name).Replace(".", string.Empty).ToLower();
                        if (ext.Contains(estensione.ToLower()))
                        {
                            logger.DebugFormat("Validazione Allegato OK! Nome: {0} , Dichiarato: {1} , Rilevato: {2} ", fd.name, estensione, ext);
                            return false;
                        }
                        else
                        {
                            logger.Debug(String.Format("Validazione file Errata {0}, Declared :[{1}]  Found:[{2}]", fd.name, estensione, ext));
                        }

                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Validazione Allegato : Eccezione {0} {1}", ex.Message, ex.StackTrace);
                    return true;
                };

            }
            return true;
        }

        private bool verificaTipoFileAll(string docNumber, InfoUtente infoUtente)
        {
            if (infoUtente == null)
                return false;

            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
            DocsPaVO.documento.FileDocumento fd = null;

            if (sch.inCestino == null)
                sch.inCestino = string.Empty;


            if (sch.inCestino != "1")
            {
                try
                {
                    //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                    if (sch.documenti != null && sch.documenti[0] != null &&
                            Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];

                        fd = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                        if (fd == null)
                            throw new Exception("Errore nel reperimento del file principale.");

                        logger.DebugFormat("Validazione Start {0}", fd.name);
                        Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
                        string ext = ff.FileType(fd.content).ToLower();

                        if (ff.isExecutable(ext))
                        {
                            logger.DebugFormat("Validazione Allegato fallita, riscontrato codice eseguibile in {0}", fd.name);
                            return true;
                        }

                        string estensione = Path.GetExtension(fd.name).Replace(".", string.Empty).ToLower();
                        if (ext.Contains(estensione))
                        {
                            logger.DebugFormat("Validazione Allegato OK! Nome: {0} , Dichiarato: {1} , Rilevato: {2} ", fd.name, estensione, ext);
                            return false;
                        }
                        else
                        {
                            logger.Debug(String.Format("Validazione Allegato Errata {0}, Declared :[{1}]  Found:[{2}]", fd.name, estensione, ext));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Validazione Allegato : Eccezione {0} {1}", ex.Message, ex.StackTrace);
                    return true;
                };

            }
            return true;
        }

        /// <summary>
        /// Restituisce l'oggetto info utente relativo all'istanza di conservazione passata come parametro
        /// </summary>
        /// <param name="infoCons">istanza di conservazione</param>
        /// <returns></returns>
        public InfoUtente getInfoUtenteConservazione(InfoConservazione infoCons)
        {
            InfoUtente result;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.getInfoUtenteConservazioneByInfoCons(infoCons);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getItemsConservazioneByIdCons", e);
                result = null;
            }
            return result;

        }

        #endregion


        /// <summary>
        /// Ritorna la lista degli item associati all'id dell'istanza passato
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ItemsConservazione> getItemsConservazioneByIdCons(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<ItemsConservazione> result;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.getItemsConservazioneByIdCons(idConservazione, infoUtente);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getItemsConservazioneByIdCons", e);
                result = null;
            }
            return result;

        }


        /// <summary>
        /// ritorna con parametri di output l'utente propietario e il ruolo di un ogetto (sia che sia documento sia che sia Fascicolo)
        /// </summary>
        /// <param name="idThing"></param>
        /// <param name="utenteProprietario"></param>
        /// <param name="ruoloProprietario"></param>
        public void getUtenteAndRuoloProprietario(String idThing, out String utenteProprietario, out String ruoloProprietario)
        {
            string ut = string.Empty;
            string ruolo = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                doc.getUtenteAndRuoloProprietario(idThing, out ut, out ruolo);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getUtenteAndRuoloProprietario", e);
                throw new Exception("Errore validazione");
            }
            utenteProprietario = ut;
            ruoloProprietario = ruolo;
        }

        /// <summary>
        /// inserisce un elemento della ReportFormatiConservazione
        /// </summary>
        /// <param name="id_Istanza"></param>
        /// <param name="id_Item"></param>
        /// <param name="docNumber"></param>
        /// <param name="id_Project"></param>
        /// <param name="version_Id"></param>
        /// <param name="tipoFile"></param>
        /// <param name="esito"></param>
        /// <param name="ammesso"></param>
        /// <param name="valido"></param>
        /// <param name="convertibile"></param>
        /// <param name="modifica"></param>
        /// <param name="ut_Prop"></param>
        /// <param name="ruolo_Prop"></param>
        /// <returns></returns>
        public string InsertItemReportFormatiConservazione(String id_Istanza,
                                                    String id_Item,
                                                    String docNumber,
                                                    String id_Project,
                                                    String version_Id,
                                                    String tipoFile,
                                                    String esito,
                                                    String ammesso,
                                                    String valido,
                                                    String convertibile,
                                                    String modifica,
                                                    String ut_Prop,
                                                    String ruolo_Prop)
        {
            string result;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.InsertItemReportFormatiConservazione(id_Istanza, id_Item, docNumber, id_Project, version_Id, tipoFile, esito, convertibile, modifica, ut_Prop, ruolo_Prop);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: InsertItemReportFormatiConservazione", e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// aggiorna l'elemento passato
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        public bool UpdateItemReportFormatiConservazione(ReportFormatiConservazione elemento)
        {

            bool result;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.UpdateItemReportFormatiConservazione(elemento);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: UpdateItemReportFormatiConservazione", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Inserisce una lista di ReportFormatiConservazione 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool InsertListItemReportFormatiConservazione(List<ReportFormatiConservazione> list)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                foreach (ReportFormatiConservazione element in list)
                {
                    result &= Cons.InsertItemReportFormatiConservazione(element);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: InsertItemReportFormatiConservazione", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// elimina tutti gli ReportFormatiConservazione associati all'idconsercazione passato
        /// </summary>
        /// <param name="idCons"></param>
        /// <returns></returns>
        public bool DeleteItemReportFormatiConservazioneByIdIstCons(String idCons)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.DeleteItemReportFormatiConservazioneByIdIstCons(idCons);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: DeleteItemReportFormatiConservazioneByIdIstCons", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aggiorna lo stato dell'istanza con esito della verifica
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="stato"></param>
        /// <param name="esitoVerifica"></param>
        /// <returns></returns>
        public bool updateStatoConservazioneWhithEsitoVerifica(string idConservazione, string stato, int esitoVerifica)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.updateStatoConservazioneWhithEsitoVerifica(idConservazione, stato, esitoVerifica);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: updateStatoConservazioneWhithEsitoVerifica", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aggiorna lo stato dell'istanza
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="stato"></param>
        /// <returns></returns>
        public bool updateStatoConservazione(string idConservazione, string stato)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.updateStatoConservazione(idConservazione, stato);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: updateStatoConservazione", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aggiorna lo stato dell'istanza
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="stato"></param>
        /// <param name="tipo_cons"></param>
        /// <param name="note"></param>
        /// <param name="descr"></param>
        /// <param name="idTipoSupp"></param>
        /// <param name="consolida"></param>
        /// <returns></returns>
        public bool updateStatoConservazione(string idConservazione, string stato, string tipo_cons, string note, string descr, string idTipoSupp, bool consolida)
        {
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.updateStatoConservazione(idConservazione, stato, tipo_cons, note, descr, idTipoSupp, consolida);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: updateStatoConservazione", e);
                result = false;
            }
            return result;
        }

        public bool checkItemConsolidato(string docNumber)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.checkItemConsolidato(docNumber);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkItemConsolidato", e);
                throw new Exception("Errore verifica consolidamento");
            }
            return result;
        }

        /// <summary>
        /// ritorna la lista di reportFormatiCosnservazione associati ad un'istanza
        /// </summary>
        /// <param name="idCons"></param>
        /// <returns></returns>
        public List<ReportFormatiConservazione> getItemReportFormatiConservazioneByIdIstCons(string idCons)
        {
            List<ReportFormatiConservazione> result;

            DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            result = Cons.getItemReportFormatiConservazioneByIdIstCons(idCons);

            return result;
        }

        /// <summary>
        /// ritorna il reportFormatiCosnservazione associato ad un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public ReportFormatiConservazione getReportFormatiConservazioneByDocNumber(string docNumber)
        {
            ReportFormatiConservazione result = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.getReportFormatiConservazioneByDocNumber(docNumber);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getReportFormatiConservazioneByDocNumber", e);
                result = null;
            }
            return result;
        }

        public void checkReportFormatiConservazioneAndUpdateStatoIstanzaCons(ReportFormatiConservazione document, string newVersionId)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.checkReportFormatiConservazioneAndUpdateStatoIstanzaCons(document, newVersionId);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: checkReportFormatiConservazioneAndUpdateStatoIstanzaCons", e);

            }
            //return result;
        }

        /// <summary>
        /// ritorna la lista dei ReportFormatiConservazione associati ad un'istanza con i dati compelti neccesari per il report
        /// </summary>
        /// <param name="idIstanzaCons"></param>
        /// <returns></returns>
        public List<ReportFormatiConservazione> getDettaglioListReportFormatiConservazioneByIdCons(string idIstanzaCons)
        {
            List<ReportFormatiConservazione> result = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.getDettaglioListReportFormatiConservazioneByIdCons(idIstanzaCons);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: UpdateItemReportFormatiConservazione", e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// ritorna la lista delle instanze per stato
        /// </summary>
        /// <param name="idIstanzaCons"></param>
        /// <returns></returns>
        public List<InfoConservazione> getInfoConservazioneDaStato(string stato)
        {
            List<InfoConservazione> result = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Conservazione Cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = Cons.GetInfoConservazioneDaStato(stato);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ConservazioneManager  - metodo: getInfoConservazioneDaStato", e);
                result = null;
            }
            return result;
        }


        public FileDocumento createReport(DocsPaVO.filtri.FiltroRicerca[] filtriReport, DataSet dataSet,
                                string tipoRep, string titoloReport, string Sottotitolo, string reportKey, string contextName, InfoUtente infoUt)
        {

            DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();

            try
            {

                DocsPaVO.Report.ReportTypeEnum rt = new DocsPaVO.Report.ReportTypeEnum();

                if (!string.IsNullOrEmpty(tipoRep) && tipoRep.Equals("PDF"))
                    rt = DocsPaVO.Report.ReportTypeEnum.PDF;
                else
                    rt = DocsPaVO.Report.ReportTypeEnum.Excel;

                List<DocsPaVO.filtri.FiltroRicerca> filtri = new List<DocsPaVO.filtri.FiltroRicerca>();

                if (filtriReport != null)
                {
                    for (int i = 0; i < filtriReport.Length; i++)
                    {
                        filtri.Add(filtriReport[i]);
                    }
                }
                DocsPaVO.Report.PrintReportRequest request;
                //String Lista = GetListaFiltri(filtri);
                if (dataSet != null)
                {
                    request = new DocsPaVO.Report.PrintReportRequestDataset()
                    {
                        UserInfo = infoUt,
                        SearchFilters = filtri,
                        Title = titoloReport,
                        SubTitle = Sottotitolo,
                        ReportType = rt,
                        ReportKey = reportKey,
                        ContextName = contextName,
                        AdditionalInformation = String.Empty,
                        InputDataset = dataSet
                    };
                }
                else
                {
                    request = new DocsPaVO.Report.PrintReportRequest()
                    {
                        UserInfo = infoUt,
                        SearchFilters = filtri,
                        Title = titoloReport,
                        SubTitle = Sottotitolo,
                        ReportType = rt,
                        ReportKey = reportKey,
                        ContextName = contextName,
                        AdditionalInformation = String.Empty
                    };
                }

                fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;
            }
            catch (Exception e)
            {
                throw e;
            }
            return fd;
        }


        #region INTEGRAZIONE PITRE-PARER

        public enum TipologiaUnitaDocumentaria
        {
            DocProtocollato,
            DocRepertoriato,
            DocNP,
            Registro,
            FatturaElettronica,
            LottoDiFatture,
            VerbaleSinteticoDiSeduta
        }

        public string getStatoConservazione(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getStatoConservazione(idDoc);
        }

        public string insertDocInCons(string idDoc, InfoUtente infoUtente)
        {
            string result = string.Empty;

            try
            {

                // controllo stato di conservazione
                string stato = this.getStatoConservazione(idDoc);

                // è possibile inserire manualmente in coda solo documenti in stato NON CONSERVATO, RIFIUTATO o VERSAMENTO FALLITO
                // le policy agiscono solo su documenti NON CONSERVATI - questo filtro è trasparente durante l'esecuzione di una policy
                if (stato.Equals("N") || stato.Equals("R") || stato.Equals("F"))
                {

                    result = this.AddDocToQueueConservazione(idDoc, infoUtente, stato);

                    // MEV Policy e responsabile conservazione
                    // In seguito all'inserimento in coda deve essere data visibilità del documento al responsabile della conservazione (se configurato)
                    if (result.Equals("OK"))
                    {
                        string idRuoloResp = this.GetIdRuoloRespConservazione(infoUtente.idAmministrazione);
                        if (!string.IsNullOrEmpty(idRuoloResp))
                        {
                            this.SetVisibilitaRuoloResp(idDoc, idRuoloResp, infoUtente.idGruppo);
                        }
                    }
                    
                }
                else
                {
                    // I documenti in stato ERRORE NELL'INVIO sono considerati in coda di versamento fin quando esauriscono il numero di tentativi di invio concessi
                    if (stato.Equals("V") || stato.Equals("T") || stato.Equals("W") || stato.Equals("E"))
                        result = "IN_QUEUE";
                    else if (stato.Equals("C"))
                        result = "DOC_CONS";
                    else
                        result = "STATE_ERR";
                }
            }
            catch (Exception ex)
            {
                result = "CONS_ERR";
                logger.Debug(ex.Message);
            }

            return result;

        }

        private string AddDocToQueueConservazione(string idDoc, InfoUtente infoUtente, string stato)
        {
            string result = string.Empty;

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            if (stato.Equals("N"))
            {
                if (!cons.addDocToQueueCons(idDoc, infoUtente))
                    result = "INS_ERR";
                else
                    result = "OK";
            }
            else
            {
                if (!cons.updateQueueCons(idDoc, infoUtente, "V", false, string.Empty, "NULL"))
                    result = "INS_ERR";
                else
                    result = "OK";
            }


            return result;
        }

        public string getFileRisposta(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getFileXML(idDoc, "VAR_FILE_RISPOSTA");
        }

        public string getFileMetadati(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getFileXML(idDoc, "VAR_FILE_METADATI");
        }

        public void ExecuteVersamento()
        {
            // inserire una chiave per accendere/spegnere il processo?

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            // ricerca id da versare
            ArrayList docToSend = new ArrayList();
            docToSend = this.getDocInQueue();

            bool result = false;

            if (docToSend != null && docToSend.Count > 0)
            {
                logger.Info(string.Format("{0} documenti in coda di versamento.", docToSend.Count));
                int counter = 1;

                // URL del web service del servizio di versamento
                string uri = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_VERSAMENTO_URL");

                if (string.IsNullOrEmpty(uri))
                {
                    logger.Info("Versamento non effettuato: errore nel reperimento dell'URL del servizio");
                }
                else
                {
                    // aggiorno la coda
                    cons.updateQueueConsList(docToSend, "W");

                    foreach (ItemsVersamento item in docToSend)
                    {
                        logger.Info(string.Format("Inizio versamento per ID={0} ({1} di {2})", item.idProfile, counter, docToSend.Count));

                        InfoUtente utente = this.getInfoUtenteVersamento(item);
                        if (utente != null)
                        {

                            string versamentoUserName = this.getConfigKey(item.idAmm, "BE_VERSAMENTO_USER");
                            string versamentoPwd = this.getConfigKey(item.idAmm, "BE_VERSAMENTO_PWD");
                            string versamentoVer = this.getConfigKey(item.idAmm, "BE_VERSAMENTO_CURR_VER");

                            // MEV Gestione doc in stato errore
                            // Numero massimo tentativi di invio
                            string maxTentativi = this.getConfigKey(item.idAmm, "BE_VERSAMENTO_MAX_TENTATIVI");
                            string maxTentativiStampe = this.getConfigKey(item.idAmm, "BE_VERSAMENTO_MAX_T_STAMPE");


                            // se non definita imposto la versione del sistema a 1.3
                            if (string.IsNullOrEmpty(versamentoVer))
                                versamentoVer = "1.3";

                            if (string.IsNullOrEmpty(versamentoUserName) || string.IsNullOrEmpty(versamentoPwd))
                            {
                                logger.Info("Errore nel reperimento delle credenziali per il versamento");
                                result = false;
                            }

                            result = this.VersamentoDoc(item, utente, versamentoUserName, versamentoPwd, versamentoVer, uri, maxTentativi, maxTentativiStampe);
                            counter++;
                            if (result)
                                logger.Info("RESULT: OK");
                            else
                                logger.Info("RESULT: KO");
                        }
                        else
                        {
                            logger.Info("Versamento non effettuato: errore nel reperimento dell'utente autore del versamento");
                        }
                    }
                }
            }
            else
            {
                logger.Info("Nessun documento da versare");
            }

        }

        public bool VersamentoDoc(ItemsVersamento item, InfoUtente utente, string userName, string pwd, string version, string uri, string maxTentativi, string maxTentativiStampe)
        {

            // valore da restituire
            bool retVal = false;

            // esito versamento
            string cha_esito = string.Empty;
            string cha_warning = string.Empty;

            bool isDocConservato = false;
            bool isStampa = false;

            string idDoc = item.idProfile;

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            

            try
            {

                // recupero scheda doc
                SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, idDoc);

                if (sch != null)
                {
   
                    isStampa = (sch.tipoProto.Equals("R") || sch.tipoProto.Equals("C"));

                    Dictionary<string, FileDocumento> filesToSend = new Dictionary<string, FileDocumento>();

                    // XML metadati
                    XmlDocument xmlMeta = this.createXMLMetadati(sch, utente);
                    //xmlMeta.Save("C:\\Windows\\Temp\\test_meta.xml"); // TEMP

                    // inserimento in DB file metadati
                    bool result = cons.insertFileXML("DPA_VERSAMENTO", idDoc, "VAR_FILE_METADATI", xmlMeta.InnerXml);
                    if (!result)
                        throw new Exception("Errore nel salvataggio del file dei metadati PITre");

                    // XML versamento
                    XmlDocument xmlDoc = this.createXMLDoc(sch, utente, out filesToSend);
                    if (filesToSend.Equals(null))
                        throw new Exception("Errore nel reperimento dei file da versare");
                    
                    logger.Debug("----- CHIAMATA XML -----");
                    logger.Debug(xmlDoc.InnerXml);
                    logger.Debug("----- fine CHIAMATA XML -----");

                    // aggiungo nel dictionary
                    /*
                    foreach (KeyValuePair<string, FileDocumento> pair in filesToSend)
                    {
                        if (pair.Value.nomeOriginale.Equals("Metadati PITre.xml"))
                        {
                            pair.Value.content = Encoding.UTF8.GetBytes(xmlMeta.InnerXml);
                            pair.Value.length = pair.Value.content.Length;
                        }
                    }
                     * */

                    // impostazione parametri request
                    NameValueCollection formFields = new NameValueCollection();
                    formFields.Add("VERSIONE", version);
                    formFields.Add("LOGINNAME", userName);
                    formFields.Add("PASSWORD", pwd);
                    formFields.Add("XMLSIP", this.replaceSpecialChars(xmlDoc.InnerXml));

                    // URL del web service del servizio di versamento
                    //string uri = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_VERSAMENTO_URL");
                    //if (string.IsNullOrEmpty(uri))
                    //    throw new Exception("Impossibile recuperare l'URL del servizio di versamento");

                    string res = this.sendRequest(formFields, filesToSend, uri);
                    XmlDocument xmlResult = new XmlDocument();
                    if (!string.IsNullOrEmpty(res))
                    {
                        if (res.Equals("TIMEOUT"))
                        {
                            // timeout nella richiesta
                            cha_esito = "T";
                        }
                        else
                        {
                            xmlResult.LoadXml(res);
                            //xmlResult.Save("C:\\Windows\\Temp\\test_result.xml"); // TEMP

                            // recupero esito versamento
                            string esito = string.Empty;
                            XmlElement esitoElement = (XmlElement)xmlResult.SelectSingleNode("EsitoVersamento/EsitoGenerale/CodiceEsito");
                            if (esitoElement != null)
                                esito = esitoElement.InnerText.Trim();

                            logger.Info("Esito versamento: " + esito);

                            // salvataggio in DB
                            if (esito.ToUpper().Equals("POSITIVO"))
                            {
                                cha_esito = "C";
                                cha_warning = "0";
                            }
                            else if (esito.ToUpper().Equals("WARNING"))
                            {
                                cha_esito = "C";
                                cha_warning = "1";
                            }
                            else
                            {
                                logger.Debug("Controllo esito...");
                                XmlElement el = (XmlElement)xmlResult.SelectSingleNode("EsitoVersamento/EsitoGenerale/CodiceErrore");
                                if (el != null)
                                {
                                    string codErr = el.InnerText.Trim();
                                    if (codErr.ToUpper().Equals("UD-002-001"))
                                    {
                                        logger.Debug("Documento già presente!");
                                        
                                        if (!string.IsNullOrEmpty(item.stato) && item.stato.Equals("T"))
                                        {
                                            // Se il documento si trovava nello stato TIMEOUT e viene inviato nuovamente
                                            // è possibile che l'operazione sia già stata portata a termine dal PARER
                                            // e pertanto il versamento ha esito negativo perché la chiave versata è già presente nel sistema.
                                            // In questo caso è necessario verificare il codice errore e, se corrisponde a chiave già presente,
                                            // impostare lo stato di conservazione come PRESO IN CARICO.
                                            cha_esito = "C";
                                        }
                                        else
                                        {
                                            // Controllo per prevenire scrittura in caso di versamento accidentale di un documento già versato
                                            isDocConservato = true;
                                            logger.Debug("Documento già presente nel sistema di conservazione");
                                        }
                                    }
                                    else
                                    {
                                        cha_esito = "R";
                                    }
                                }
                            }
                            if (!isDocConservato)
                            {
                                logger.Debug("Aggiornamento coda");
                                result = cons.updateQueueCons(idDoc, utente, cha_esito, true, cha_warning, string.Empty);
                                result = cons.insertFileXML("DPA_VERSAMENTO", idDoc, "VAR_FILE_RISPOSTA", xmlResult.InnerXml);
                            }
                            else
                            {
                                result = true;
                            }
                            if (!result)
                                throw new Exception("Errore nel salvataggio del file XML di risposta.");
                        }
                        //if (!result)
                        //    throw new Exception("Errore nell'aggiornamento della coda di versamento.");
                    }

                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                logger.Debug("Errore nel versamento: ", ex);
                this.SaveErrorMessage(idDoc, ex.ToString());
                cha_esito = "E";
                cha_warning = string.Empty;
            }
            finally
            {
                if (!isDocConservato)
                {
                    // In caso di documenti per i quali si è verificato un errore aggiorno il numero di tentativi di invio
                    string numTentativi = string.Empty;
                    if (cha_esito.Equals("E") || cha_esito.Equals("T"))
                    {
                        if (!string.IsNullOrEmpty(item.tentativiInvio))
                        {
                            int t = 0;
                            int max = 0;
                            t = Convert.ToInt32(item.tentativiInvio) + 1;
                            numTentativi = t.ToString();

                            if (!isStampa)
                            {
                                if (!string.IsNullOrEmpty(maxTentativi))
                                {
                                    if (Int32.TryParse(maxTentativi, out max))
                                    {
                                        if (max > 0 && t >= max)
                                        {
                                            // Si è raggiunto il numero massimo di tentativi di invio per il documento
                                            // Modifico lo stato in "Versamento fallito
                                            cha_esito = "F";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(maxTentativiStampe))
                                {
                                    if (Int32.TryParse(maxTentativiStampe, out max))
                                    {
                                        if (max > 0 && t >= max)
                                        {
                                            // Si è raggiunto il numero massimo di tentativi di invio per il documento
                                            // Modifico lo stato in "Versamento fallito
                                            cha_esito = "F";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            numTentativi = "1";
                        }
                    }

                    // aggiorno la coda di versamento
                    bool result = cons.updateQueueCons(idDoc, utente, cha_esito, true, cha_warning, numTentativi);

                    // se è avvenuta la presa in carico verifico lo stato di consolidamento e, se necessario, consolido il documento
                    if (cha_esito.Equals("C"))
                    {
                        if (!BusinessLogic.Documenti.DocumentConsolidation.IsDocumentConsoldated(utente, idDoc, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2))
                        {
                            logger.Debug("Consolidamento doc id=" + idDoc);
                            try
                            {
                                DocsPaVO.documento.DocumentConsolidationStateInfo state = new DocumentConsolidationStateInfo();
                                //state = BusinessLogic.Documenti.DocumentConsolidation.Consolidate(utente, idDoc, DocumentConsolidationStateEnum.Step2);
                                //state = BusinessLogic.Documenti.DocumentConsolidation.Consolidate(utente, idDoc, DocumentConsolidationStateEnum.Step2, true);
                                state = BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(utente, idDoc, DocumentConsolidationStateEnum.Step2, true);
                                logger.Debug("Termine consolidamento");
                            }
                            catch (Exception exc)
                            {
                                logger.Debug("Errore nel consolidamento");
                            }
                        }
                    }

                    // inserimento log
                    if (cha_esito.Equals("C"))
                        BusinessLogic.UserLog.UserLog.WriteLog(utente, "VERSAMENTO_DOC", idDoc, "Versamento documento ID=" + idDoc, DocsPaVO.Logger.CodAzione.Esito.OK);
                    else
                        BusinessLogic.UserLog.UserLog.WriteLog(utente, "VERSAMENTO_DOC", idDoc, "Versamento documento ID=" + idDoc, DocsPaVO.Logger.CodAzione.Esito.KO);
                }
            }

            return retVal;

        }

        public string RecuperoDoc(string idDoc, InfoUtente utente)
        {
            string result = string.Empty;

            try
            {
                // recupero scheda doc
                SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, idDoc);

                // recupero credenziali per versamento
                string versamentoUserName = this.getConfigKey(utente.idAmministrazione, "BE_VERSAMENTO_USER");
                string versamentoPwd = this.getConfigKey(utente.idAmministrazione, "BE_VERSAMENTO_PWD");

                string uri = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RECUPERO_URL");

                if (string.IsNullOrEmpty(versamentoUserName) || string.IsNullOrEmpty(versamentoPwd) || string.IsNullOrEmpty(uri))
                    throw new Exception("Errore nel reperimento dei parametri di configurazione PARER");

                // impostazione parametri request
                NameValueCollection formFields = new NameValueCollection();
                formFields.Add("VERSIONE", "1.0");
                formFields.Add("LOGINNAME", versamentoUserName);
                formFields.Add("PASSWORD", versamentoPwd);
                formFields.Add("XMLSIP", this.replaceSpecialChars(this.createXMLRecupero(sch, utente)));

                result = this.sendRequest(formFields, null, uri);
                
                if (result.Equals("TIMEOUT"))
                    throw new Exception("Timeout nella richiesta");            

            }
            catch (Exception ex)
            {
                result = string.Empty;
                logger.Debug(ex.Message);
            }

            return result;
        }

        public string GetRapportoVersamento(string id, InfoUtente utente)
        {
            string isConservazionePARER = this.getConfigKey(utente.idAmministrazione, "FE_WA_CONSERVAZIONE");

            if (!string.IsNullOrEmpty(isConservazionePARER) && isConservazionePARER.Equals("1"))
            {
                return this.GetRapportoVersamentoPARER(id, utente);
            }
            else
            {
                return this.GetRapportoVersamentoDocsPA(id, utente);
            }
        }

        private string GetRapportoVersamentoPARER(string idDoc, InfoUtente utente)
        {

            string retVal = string.Empty;

            try
            {
                // estraggo il file di risposta
                string xmlString = this.getFileRisposta(idDoc);
                if (!string.IsNullOrEmpty(xmlString))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(xmlString);

                    string stato = this.getStatoConservazione(idDoc);
                    string rapporto = string.Empty;

                    // Documento preso in carico
                    // Restituisco il rapporto di versamento
                    if (stato.Equals("C"))
                    {

                        if (xml.SelectSingleNode("EsitoVersamento/RapportoVersamento") != null)
                        {
                            rapporto = xml.SelectSingleNode("EsitoVersamento/RapportoVersamento").InnerXml;
                            if (!string.IsNullOrEmpty(rapporto))
                            {
                                retVal = HttpUtility.HtmlDecode(rapporto);
                            }
                        }
                    }
                    // Documento rifiutato
                    // Restituisco il file XML di risposta
                    else
                    {
                        retVal = xml.InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                retVal = string.Empty;
            }

            return retVal;

        }

        private string GetRapportoVersamentoDocsPA(string idIstanza, InfoUtente utente)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetRapportoVersamentoDocsPA(idIstanza, utente);
        }

        private string createXMLRecupero(SchedaDocumento doc, InfoUtente utente)
        {
            string result = string.Empty;

            XmlDocument xml = new XmlDocument();

            XmlNode node = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode recupero = xml.CreateElement("Recupero");
            XmlElement versione = xml.CreateElement("Versione");
            XmlElement versatore = xml.CreateElement("Versatore");
            XmlElement chiave = xml.CreateElement("Chiave");
            XmlElement ambiente = xml.CreateElement("Ambiente");
            XmlElement ente = xml.CreateElement("Ente");
            XmlElement struttura = xml.CreateElement("Struttura");
            XmlElement userId = xml.CreateElement("UserID");
            XmlElement numero = xml.CreateElement("Numero");
            XmlElement anno = xml.CreateElement("Anno");
            XmlElement tipoRegistro = xml.CreateElement("TipoRegistro");

            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione);
            DocsPaVO.utente.Registro reg = null;
            if (doc.registro != null)
            {
                reg = BusinessLogic.Utenti.RegistriManager.getRegistro(doc.registro.systemId);
            }

            // VALORI CABLATI!!
            versione.InnerText = "1.0";
            //ambiente.InnerText = "PARER_TEST";
            //ente.InnerText = "Ente_Test_Parer";
            //struttura.InnerText = "PITre-test";
            ambiente.InnerText = this.getConfigKey(utente.idAmministrazione, "BE_VERSAMENTO_AMBIENTE");
            ente.InnerText = this.replaceSpecialCharsHeader(amm.Codice);
            struttura.InnerText = reg != null ? this.replaceSpecialChars(reg.codice) : this.replaceSpecialCharsHeader(amm.Codice);
            userId.InnerText = this.getConfigKey(utente.idAmministrazione, "BE_VERSAMENTO_USER");

            ChiaveVersamento c = this.getChiaveVersamento(doc, this.getTipoDocumento(doc, amm.Codice));
            numero.InnerText = c.numero;
            anno.InnerText = c.anno;
            tipoRegistro.InnerText = c.tipoRegistro;

            versatore.AppendChild(ambiente);
            versatore.AppendChild(ente);
            versatore.AppendChild(struttura);
            versatore.AppendChild(userId);

            chiave.AppendChild(numero);
            chiave.AppendChild(anno);
            chiave.AppendChild(tipoRegistro);

            recupero.AppendChild(versione);
            recupero.AppendChild(versatore);
            recupero.AppendChild(chiave);

            xml.AppendChild(node);
            xml.AppendChild(recupero);

            return xml.InnerXml;
        }

        private ArrayList getDocInQueue()
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            ArrayList result = new ArrayList();
            
            // 1) STAMPE
            ArrayList listaStampe = cons.getListaDocToSend(true);
            if (listaStampe != null && listaStampe.Count > 0)
                result.AddRange(listaStampe);

            // 2) ALTRE TIPOLOGIE
            ArrayList lista = cons.getListaDocToSend(false);
            if (lista != null && lista.Count > 0)
                result.AddRange(lista);

            return result;
        }

        private string sendRequest(NameValueCollection formFields, Dictionary<string, FileDocumento> fileFields, string uri)
        {

            string retVal = string.Empty;

            HttpWebRequest webrequest;
            webrequest = (HttpWebRequest)WebRequest.Create(uri);

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            webrequest.Method = "POST";
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.KeepAlive = false;
            webrequest.Timeout = System.Threading.Timeout.Infinite;

            string formFieldTemplate = "Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}\r\n";

            StringBuilder sbHeader = new StringBuilder();

            // parametri da inserire nell'header
            if (formFields != null)
            {
                foreach (string key in formFields.AllKeys)
                {
                    string[] values = formFields.GetValues(key);
                    if (values != null)
                    {
                        foreach (string value in values)
                        {
                            sbHeader.AppendFormat("--{0}\r\n", boundary);
                            sbHeader.AppendFormat(formFieldTemplate, key, value);
                        }
                    }
                }
            }

            byte[] header = Encoding.UTF8.GetBytes(sbHeader.ToString());
            byte[] footer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            //byte[] footer = Encoding.ASCII.GetBytes("--\r\n");

            MemoryStream ms = new MemoryStream();
            ms.Write(header, 0, header.Length);


            //string fileFieldTemplate = "Content-Disposition: form-data; name=\"{0}\";\r\n\r\nfilename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";
            string fileFieldTemplate = "Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\";Content-Type: {2}\r\n\r\n";

            // file da inviare
            if (fileFields != null)
            {
                ms.Write(boundaryBytes, 0, boundaryBytes.Length);
                //bool firstItem = true;

                // Individuo l'ultimo elemento
                string lastId = fileFields.Last().Key;

                foreach (KeyValuePair<string, FileDocumento> kvp in fileFields)
                {

                    string CRLF = "\r\n";
                    byte[] CRLFbytes = Encoding.UTF8.GetBytes(CRLF);
                    FileDocumento fd = kvp.Value;
                    if (fd != null && fd.content != null)
                    {
                        StringBuilder sbFile = new StringBuilder();
                        sbFile.AppendFormat("--{0}\r\n", boundary);
                        sbFile.AppendFormat(fileFieldTemplate, kvp.Key, fd.fullName, "application/octet-stream");
                        //string fileHead = string.Format(fileFieldTemplate, kvp.Key, fd.fullName, "application/octet-stream");

                        byte[] headerBytes = Encoding.UTF8.GetBytes(sbFile.ToString());
                        ms.Write(headerBytes, 0, headerBytes.Length);
                        ms.Write(fd.content, 0, fd.content.Length);

                        // aggiungo il CRLF solo se non sto inserendo l'ultimo elemento
                        if (lastId != kvp.Key)
                            ms.Write(CRLFbytes, 0, CRLFbytes.Length);

                        //ms.Write(boundaryBytes, 0, boundaryBytes.Length);
                    }
                    else
                    {
                        logger.Debug("CONTENT NULL per ID=" + kvp.Key);
                    }
                }
            }

            //ms.Write(boundaryBytes, 0, boundaryBytes.Length);
            ms.Write(footer, 0, footer.Length);
            long contentLength = ms.Length;

            logger.Debug("Creazione stream richiesta....");
            webrequest.ContentLength = contentLength;
            using (Stream rs = webrequest.GetRequestStream())
            {
                ms.Position = 0;
                byte[] buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                rs.Write(buffer, 0, buffer.Length);
            }

            logger.Debug("Esecuzione chiamata...");

            try
            {
                using (var webResponse = (HttpWebResponse)webrequest.GetResponse())
                {

                    logger.DebugFormat("Risposta status code del servizio REST: {0}", webResponse.StatusCode);

                    // gestione timeout richiesta
                    if (webResponse.StatusCode.Equals(HttpStatusCode.RequestTimeout))
                    {
                        retVal = "TIMEOUT";
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            retVal = reader.ReadToEnd();
                        }
                    }

                    logger.Debug("Risposta servizio: " + retVal);
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("timed out"))
                {
                    logger.Debug("Timeout nell'operazione");
                    retVal = "TIMEOUT";
                }
                else
                {
                    switch (ex.Status)
                    {
                        case WebExceptionStatus.Timeout:
                            // Gestione per timeout richieste HTTP
                            logger.Debug("Timeout nell'operazione");
                            retVal = "TIMEOUT";
                            break;

                        case WebExceptionStatus.ReceiveFailure:
                        case WebExceptionStatus.KeepAliveFailure:
                            // Gestione per timeout richieste HTTPS.
                            // Se la richiesta è in timeout l'eccezione è gestita dalla classe interna SSL e lo stato viene modificato in ReceiveFailure o KeepAliveFailure.
                            // Per determinare se l'errore sia davvero un timeout è necessario analizzare il messaggio di errore della proprietà InnerException.
                            if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message != null)
                            {
                                string message = ex.InnerException.InnerException.Message;
                                string msg = "Impossibile stabilire la connessione. Risposta non corretta della parte connessa dopo l'intervallo di tempo oppure mancata risposta dall'host collegato";
                                string msgEn = "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond";

                                if (message.Equals(msg, StringComparison.CurrentCultureIgnoreCase) || message.Equals(msgEn, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // TIMEOUT
                                    logger.Debug("Timeout nell'operazione");
                                    retVal = "TIMEOUT";
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                throw ex;
                            }
                            break;
                        default:
                            throw ex;
                    }
                }
            }

            return retVal;
        }

        private XmlDocument createXMLDoc(SchedaDocumento doc, InfoUtente infoUt, out Dictionary<string, FileDocumento> filesToSend)
        {

            XmlDocument xml = new XmlDocument();
            filesToSend = new Dictionary<string, FileDocumento>();

            logger.Debug(string.Format("Creazione XML di versamento per il documento ID={0}", doc.systemId));

            // reperimento dati relativi all'amministrazione e all'utente creatore
            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUt.idAmministrazione);

            DocsPaVO.utente.Utente utenteCreatore = BusinessLogic.Utenti.UserManager.getUtenteById(doc.creatoreDocumento.idPeople);
            DocsPaVO.utente.Ruolo ruoloCreatore = new DocsPaVO.utente.Ruolo();
            if (!string.IsNullOrEmpty(doc.creatoreDocumento.idCorrGlob_Ruolo))
                ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(doc.creatoreDocumento.idCorrGlob_Ruolo);
            else
                ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuoliUtente(doc.creatoreDocumento.idPeople).Count > 0 ? (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getRuoliUtente(doc.creatoreDocumento.idPeople)[0] : null;

            // tipo documento
            TipologiaUnitaDocumentaria tipoDoc = this.getTipoDocumento(doc, amm.Codice);

            // caricamento template XML
            string versioneDati = string.Empty;
            switch (tipoDoc)
            {
                case TipologiaUnitaDocumentaria.DocProtocollato:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "P"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "P");
                    break;

                case TipologiaUnitaDocumentaria.DocNP:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "G"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "G");
                    break;

                case TipologiaUnitaDocumentaria.DocRepertoriato:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "R"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "R");
                    break;

                case TipologiaUnitaDocumentaria.Registro:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "S"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "S");
                    break;
                case TipologiaUnitaDocumentaria.FatturaElettronica:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "F"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "F");
                    break;
                case TipologiaUnitaDocumentaria.LottoDiFatture:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "L"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "L");
                    break;
                case TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta:
                    xml.LoadXml(this.getTemplateXML(amm.IDAmm, "V"));
                    versioneDati = this.getVersioneDatiSpecifici(amm.IDAmm, "V");
                    break;
            }

            // contatore progressivo componenti
            int componenti = 1;

            // compilazione campi comuni

            ArrayList listaRegistri = BusinessLogic.Amministrazione.RegistroManager.GetRegistri(amm.Codice, "0");
            string strutturaParer = string.Empty;

            #region MEV STRUTTURA PER ENTI MULTI AOO
            if (listaRegistri != null && listaRegistri.Count > 1 && !string.IsNullOrEmpty(this.getConfigKey(amm.IDAmm, "BE_VERSAMENTO_MULTI_AOO")) && this.getConfigKey(amm.IDAmm, "BE_VERSAMENTO_MULTI_AOO").Equals("1"))
            {
                // MULTI AOO
                switch (tipoDoc)
                {
                    case TipologiaUnitaDocumentaria.DocProtocollato:
                        strutturaParer = doc.registro.codRegistro;
                        break;
                    case TipologiaUnitaDocumentaria.DocRepertoriato:
                    case TipologiaUnitaDocumentaria.FatturaElettronica:
                    case TipologiaUnitaDocumentaria.LottoDiFatture:
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom counter = this.getContatoreRepertorio(doc);
                        if (counter != null && counter.CONSERVAZIONE == "1" && counter.CONS_REPERTORIO == "1")
                        {
                            if (counter.TIPO_CONTATORE == "A")
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(counter.ID_AOO_RF);
                                strutturaParer = reg.codRegistro;
                            }
                            else if (counter.TIPO_CONTATORE == "R")
                            {
                                DocsPaVO.utente.Registro regRF = BusinessLogic.Utenti.RegistriManager.getRegistro(counter.ID_AOO_RF);
                                if (!string.IsNullOrEmpty(regRF.idAOOCollegata))
                                {
                                    strutturaParer = BusinessLogic.Utenti.RegistriManager.getRegistro(regRF.idAOOCollegata).codRegistro;
                                }
                            }
                            else
                            {
                                // Se è protocollato utilizzo il registro
                                if (doc.registro != null && !string.IsNullOrEmpty(doc.registro.codRegistro))
                                {
                                    strutturaParer = doc.registro.codRegistro;
                                }
                            }
                            
                        }
                        break;
                    case TipologiaUnitaDocumentaria.Registro:
                        if (doc.tipoProto == "R")
                        {
                            // Stampa registro di protocollo
                            DocsPaVO.areaConservazione.StampaRegistro stampa = this.getInfoStampaRegistro(doc.systemId);
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro);
                            strutturaParer = reg.codRegistro;
                        }
                        else
                        {
                            // Stampa registro di repertorio
                            // recupero contatore
                            DocsPaVO.areaConservazione.StampaRegistro stampa = this.getInfoStampaRepertorio(doc.systemId);
                            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getOggettoById(stampa.idRepertorio);

                            if (ogg.TIPO_CONTATORE == "A")
                            {
                                strutturaParer = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro).codRegistro;
                            }
                            else if (ogg.TIPO_CONTATORE == "R")
                            {
                                DocsPaVO.utente.Registro regRF = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro);
                                if (!string.IsNullOrEmpty(regRF.idAOOCollegata))
                                {
                                    strutturaParer = BusinessLogic.Utenti.RegistriManager.getRegistro(regRF.idAOOCollegata).codRegistro;
                                }
                            }
                            // TIPOLOGIA
                        }
                        break;
                    case TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta:
                        strutturaParer = this.GetCodiceAOO(doc.systemId);
                        break;
                    default:
                        break;
                }

                // Se il parametro strutturaParer non è valorizzato dal blocco sopra
                // significa che è un documento grigio o un repertorio di tipologia (non protocollato)
                // Utilizzo il codice amm
                if (string.IsNullOrEmpty(strutturaParer))
                {
                    strutturaParer = amm.Codice;
                }
            
            }
            else
            {
                // MONO AOO
                // Si utilizza il codice amm
                strutturaParer = amm.Codice;
            }


            #endregion


            #region Intestazione

            logger.Debug("Intestazione");
            XmlNode intestazioneNode = xml.SelectSingleNode("UnitaDocumentaria/Intestazione");
            string versione = this.getConfigKey(amm.IDAmm, "BE_VERSAMENTO_CURR_VER");
            if (string.IsNullOrEmpty(versione))
                versione = "1.3";
            intestazioneNode.SelectSingleNode("Versione").InnerText = versione;

            intestazioneNode.SelectSingleNode("Versatore/Ambiente").InnerText = this.getConfigKey(amm.IDAmm, "BE_VERSAMENTO_AMBIENTE");
            intestazioneNode.SelectSingleNode("Versatore/Ente").InnerText = this.replaceSpecialCharsHeader(amm.Codice);
            intestazioneNode.SelectSingleNode("Versatore/Struttura").InnerText = strutturaParer;
            // VALORI CABLATI UTILIZZATI IN TEST
            //intestazioneNode.SelectSingleNode("Versatore/Ambiente").InnerText = "PARER_TEST";
            //intestazioneNode.SelectSingleNode("Versatore/Ente").InnerText = "PAT";
            //intestazioneNode.SelectSingleNode("Versatore/Struttura").InnerText = "PAT";
            intestazioneNode.SelectSingleNode("Versatore/UserID").InnerText = this.getConfigKey(infoUt.idAmministrazione, "BE_VERSAMENTO_USER");

            #endregion

            #region Configurazione

            logger.Debug("Configurazione");
            XmlNode configurazioneNode = xml.SelectSingleNode("UnitaDocumentaria/Configurazione");
            configurazioneNode.SelectSingleNode("TipoConservazione").InnerText = "VERSAMENTO_ANTICIPATO";
            if (tipoDoc == TipologiaUnitaDocumentaria.FatturaElettronica || tipoDoc == TipologiaUnitaDocumentaria.LottoDiFatture)
                configurazioneNode.SelectSingleNode("TipoConservazione").InnerText = "FISCALE";

            configurazioneNode.SelectSingleNode("ForzaConservazione").InnerText = "true";
            configurazioneNode.SelectSingleNode("ForzaAccettazione").InnerText = "true";
            configurazioneNode.SelectSingleNode("ForzaCollegamento").InnerText = "true";
            configurazioneNode.SelectSingleNode("SimulaSalvataggioDatiInDB").InnerText = "false";

            #endregion

            #region Profilo Archivistico

            logger.Debug("Profilo archivistico");
            ArrayList listaFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUt, doc.docNumber);

            if (listaFascicoli != null && listaFascicoli.Count > 0)
            {

                // se il documento è stato sia fascicolato che classificato, come fascicolo principale devono essere riportati
                // i dati relativi alla prima fascicolazione, anche se cronologicamente la classificazione è avvenuta prima

                string idPrimaFascicolazione = this.GetIdFascPrimaFascicolazione(doc.docNumber);

                if (!string.IsNullOrEmpty(idPrimaFascicolazione))
                {
                    foreach (Fascicolo f in listaFascicoli)
                    {
                        if (f.systemID.Equals(idPrimaFascicolazione))
                            f.isFascPrimaria = "1";
                        else
                            f.isFascPrimaria = "0";
                    }

                }
                else
                {
                    // controllo su fascicolazione primaria
                    bool existFascPrim = false;
                    foreach (Fascicolo f in listaFascicoli)
                    {
                        if (f.isFascPrimaria.Equals("1"))
                            existFascPrim = true;
                    }
                    if (!existFascPrim)
                    {
                        ((Fascicolo)listaFascicoli[0]).isFascPrimaria = "1";
                    }
                }

                XmlNode profArchivisticoNode = xml.SelectSingleNode("UnitaDocumentaria/ProfiloArchivistico");
                List<Fascicolo> listaFascSecondari = new List<Fascicolo>();

                ArrayList folders = BusinessLogic.Fascicoli.FolderManager.GetFoldersDocument(doc.docNumber);
                
                bool fascPrim = false;
                foreach (Fascicolo fascicolo in listaFascicoli)
                {
                    
                    if (!string.IsNullOrEmpty(fascicolo.isFascPrimaria) && fascicolo.isFascPrimaria.Equals("1") && !fascPrim)
                    {
                        fascPrim = true;

                        DocsPaVO.fascicolazione.Classifica[] gerarchia = BusinessLogic.Fascicoli.TitolarioManager.getGerarchia(fascicolo.idClassificazione, amm.IDAmm);
                        logger.Debug("gerarchia id=" + fascicolo.systemID + "  f.primario");
                        logger.Debug(gerarchia.Length);
                        profArchivisticoNode.SelectSingleNode("FascicoloPrincipale/Classifica").InnerText = gerarchia[gerarchia.Length - 1].codice;

                        // le informazioni sul fascicolo non devono essere visualizzate se il documento è solo classificato
                        if (fascicolo.tipo.Equals("P"))
                        {
                            profArchivisticoNode.SelectSingleNode("FascicoloPrincipale/Fascicolo/Identificativo").InnerText = fascicolo.codice;
                            profArchivisticoNode.SelectSingleNode("FascicoloPrincipale/Fascicolo/Oggetto").InnerText = fascicolo.descrizione;
                        }
                        else
                        {
                            profArchivisticoNode.SelectSingleNode("FascicoloPrincipale").RemoveChild(profArchivisticoNode.SelectSingleNode("FascicoloPrincipale/Fascicolo"));
                        }

                        bool hasSottoFascicolo = false;

                        // sottofascicoli
                        // Provvisoriamente commentato 04/07/2016 -------------
                        foreach (Folder folder in folders)
                        {
                            if (fascicolo.systemID.Equals(folder.idFascicolo))
                            {
                                profArchivisticoNode.SelectSingleNode("FascicoloPrincipale").RemoveChild(profArchivisticoNode.SelectSingleNode("FascicoloPrincipale/SottoFascicolo"));
                                profArchivisticoNode.SelectSingleNode("FascicoloPrincipale").AppendChild(this.AddSottoFascicoloElement(ref xml, fascicolo, folder));
                                hasSottoFascicolo = true;
                                break; // solo il primo sottofascicolo
                            }
                        }
                        // ---------------------------------------------------
                        // se il fasc principale non ha sottofascicoli rimuovo l'elemento corrispondente
                        if (!hasSottoFascicolo)
                        {
                            profArchivisticoNode.SelectSingleNode("FascicoloPrincipale").RemoveChild(profArchivisticoNode.SelectSingleNode("FascicoloPrincipale/SottoFascicolo"));
                        }
                    }
                    else
                    {
                        listaFascSecondari.Add(fascicolo);
                    }
                }

                // fascicoli secondari
                if (listaFascSecondari != null && listaFascSecondari.Count > 0)
                {
                    logger.Debug("Fascicoli secondari");
                    XmlNode fascSecondariNode = profArchivisticoNode.SelectSingleNode("FascicoliSecondari");

                    foreach (Fascicolo fascicolo in listaFascSecondari)
                    {

                        DocsPaVO.fascicolazione.Classifica[] gerarchia = BusinessLogic.Fascicoli.TitolarioManager.getGerarchia(fascicolo.idClassificazione, amm.IDAmm);
                        logger.Debug("gerarchia id=" + fascicolo.systemID);
                        logger.Debug(gerarchia.Length);
                        XmlNode fascSecNode = xml.CreateElement("FascicoloSecondario");
                        XmlElement elClassifica = xml.CreateElement("Classifica");
                        elClassifica.InnerText = gerarchia[gerarchia.Length - 1].codice;
                        fascSecNode.AppendChild(elClassifica);

                        // le informazioni sul fascicolo non devono essere visualizzate se il documento è solo classificato
                        if (fascicolo.tipo.Equals("P"))
                        {
                            XmlNode fascNode = xml.CreateElement("Fascicolo");
                            XmlElement elIdentificativo = xml.CreateElement("Identificativo");
                            XmlElement elOggetto = xml.CreateElement("Oggetto");
                            elIdentificativo.InnerText = fascicolo.codice;
                            elOggetto.InnerText = fascicolo.descrizione;
                            fascNode.AppendChild(elIdentificativo);
                            fascNode.AppendChild(elOggetto);
                            fascSecNode.AppendChild(fascNode);
                        }

                        // sottofascicoli
                        // Commentato provvioriamente 04/07/2016 -------------------
                        foreach (Folder folder in folders)
                        {
                            if (fascicolo.systemID.Equals(folder.idFascicolo))
                            {
                                fascSecNode.AppendChild(this.AddSottoFascicoloElement(ref xml, fascicolo, folder));
                                break; //solo il primo sottofascicolo
                            }
                        }
                        // ---------------------------------------------------------

                        fascSecondariNode.AppendChild(fascSecNode);
                    }
                }
                else
                {
                    profArchivisticoNode.RemoveChild(profArchivisticoNode.SelectSingleNode("FascicoliSecondari"));
                }

            }
            else
            {
                xml.SelectSingleNode("UnitaDocumentaria").RemoveChild(xml.SelectSingleNode("UnitaDocumentaria/ProfiloArchivistico"));
            }

            #endregion

            #region Profilo Unità Documentaria

            logger.Debug("Profilo UD");
            XmlNode profiloUDNode = xml.SelectSingleNode("UnitaDocumentaria/ProfiloUnitaDocumentaria");
            profiloUDNode.SelectSingleNode("Oggetto").InnerText = doc.oggetto.descrizione;

            #endregion

            #region Dati Specifici

            logger.Debug("Dati specifici");
            XmlNode datiSpecificiNode = xml.SelectSingleNode("UnitaDocumentaria/DatiSpecifici");
            datiSpecificiNode.SelectSingleNode("VersioneDatiSpecifici").InnerText = versioneDati;
            if (tipoDoc != TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta)
            {
                datiSpecificiNode.SelectSingleNode("DataCreazioneProfiloDocumento").InnerText = this.formatDate(doc.dataCreazione);
                datiSpecificiNode.SelectSingleNode("UtenteCreatore").InnerText = utenteCreatore.descrizione;
                //datiSpecificiNode.SelectSingleNode("RuoloUtenteCreatore").InnerText = ruoloCreatore.descrizione;
                if (ruoloCreatore != null)
                    datiSpecificiNode.SelectSingleNode("RuoloCreatore").InnerText = ruoloCreatore.descrizione;
                else
                    datiSpecificiNode.SelectSingleNode("RuoloCreatore").InnerText = "Ruolo non definito";
                if (ruoloCreatore != null && ruoloCreatore.uo != null)
                    datiSpecificiNode.SelectSingleNode("UOCreatrice").InnerText = ruoloCreatore.uo.descrizione;
                else
                    datiSpecificiNode.SelectSingleNode("UOCreatrice").InnerText = "Struttura non presente";
            }
            #endregion

            #region Documenti Collegati

            logger.Debug("Documenti collegati");
            if (doc.rispostaDocumento != null && (!string.IsNullOrEmpty(doc.rispostaDocumento.segnatura) || !string.IsNullOrEmpty(doc.rispostaDocumento.docNumber)))
            {

                XmlNode docCollegatiNode = xml.SelectSingleNode("UnitaDocumentaria/DocumentiCollegati");

                XmlElement docColl = xml.CreateElement("DocumentoCollegato");

                XmlElement chiave = xml.CreateElement("ChiaveCollegamento");
                XmlElement descrizione = xml.CreateElement("DescrizioneCollegamento");
                descrizione.InnerText = "Catena documentale";

                XmlElement numero = xml.CreateElement("Numero");
                XmlElement anno = xml.CreateElement("Anno");
                XmlElement tipoReg = xml.CreateElement("TipoRegistro");

                // scheda documento collegato
                SchedaDocumento docCollegato = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUt, doc.rispostaDocumento.docNumber);

                // tipo doc collegato
                TipologiaUnitaDocumentaria tipoDocColl = this.getTipoDocumento(docCollegato, amm.Codice);

                ChiaveVersamento key = this.getChiaveVersamento(docCollegato, tipoDocColl);
                numero.InnerText = key.numero;
                anno.InnerText = key.anno;
                tipoReg.InnerText = key.tipoRegistro;


                /*
                if (tipoDocColl == TipologiaUnitaDocumentaria.DocProtocollato)
                {
                    numero.InnerText = docCollegato.protocollo.numero;
                    anno.InnerText = docCollegato.protocollo.anno;
                    tipoReg.InnerText = string.Format("{0} - Protocollo",docCollegato.registro.codRegistro);
                }
                if (tipoDocColl == TipologiaUnitaDocumentaria.DocRepertoriato)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggContatore = this.getContatoreRepertorio(docCollegato);

                    string data = string.Empty;
                    //numero.InnerText = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, amm.Codice, false, out data);
                    numero.InnerText = oggContatore.VALORE_DATABASE;
                    anno.InnerText = oggContatore.ANNO;

                    string appo = docCollegato.template.DESCRIZIONE;
                    if (oggContatore.TIPO_CONTATORE.Equals("A"))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggContatore.ID_AOO_RF);
                        appo = reg.codRegistro + " - " + appo;
                    }
                    if (oggContatore.TIPO_CONTATORE.Equals("R"))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggContatore.ID_AOO_RF);
                        appo = reg.codRegistro + " - " + appo;
                    }

                    tipoReg.InnerText = appo;

                }
                if (tipoDocColl == TipologiaUnitaDocumentaria.DocNP)
                {
                    numero.InnerText = docCollegato.docNumber;
                    anno.InnerText = this.getDate(docCollegato.dataCreazione.Trim()).Split('/').Last();
                    tipoReg.InnerText = string.Format("{0} - PITre", amm.Descrizione);
                }
                if (tipoDocColl == TipologiaUnitaDocumentaria.Registro)
                {

                }
                */

                if (tipoDocColl == TipologiaUnitaDocumentaria.LottoDiFatture)
                {
                    descrizione.InnerText = "Appartenenza a lotto";
                }

                chiave.AppendChild(numero);
                chiave.AppendChild(anno);
                chiave.AppendChild(tipoReg);

                docColl.AppendChild(chiave);
                docColl.AppendChild(descrizione);

                docCollegatiNode.AppendChild(docColl);

            }
            else
            {
                if (!(tipoDoc.Equals(TipologiaUnitaDocumentaria.DocRepertoriato) && !doc.tipoProto.Equals("G")))
                {
                    xml.SelectSingleNode("UnitaDocumentaria").RemoveChild(xml.SelectSingleNode("UnitaDocumentaria/DocumentiCollegati"));
                }

            }

            #endregion

            #region Documento Principale

            logger.Debug("Documento principale");
            DocsPaVO.documento.Documento docPrincipale = (DocsPaVO.documento.Documento)doc.documenti[0];
            DocsPaVO.documento.Documento docScambiato = new DocsPaVO.documento.Documento();
            DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
            bool scambioFattura = false, erroreScambio=false, erroreDoppioScambio=false;
            XmlNode docPrincipaleNode = xml.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale");
            string idPr = doc.docNumber + "_C" + componenti.ToString();
            string nomeFileFatt = "", nomeFileRappr = "", nomeAllegato = "" ;
            docPrincipaleNode.SelectSingleNode("IDDocumento").InnerText = doc.systemId;
            string docPrincDesc = "Documento Principale";
            if (tipoDoc == TipologiaUnitaDocumentaria.LottoDiFatture)
                docPrincDesc = "LOTTO DI FATTURE";
            if (tipoDoc == TipologiaUnitaDocumentaria.FatturaElettronica)
            {
                if (doc.template != null && !string.IsNullOrEmpty(doc.template.DESCRIZIONE))
                {
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in doc.template.ELENCO_OGGETTI)
                    {
                        if (oggetto.DESCRIZIONE.ToUpper() == "TIPO ATTO") docPrincDesc = oggetto.VALORE_DATABASE;
                    }
                }
            }
            if (tipoDoc == TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta)
                docPrincDesc = "VERBALE SINTETICO DI SEDUTA";

            docPrincipaleNode.SelectSingleNode("TipoDocumento").InnerText = docPrincDesc;

            docPrincipaleNode.SelectSingleNode("ProfiloDocumento/Descrizione").InnerText = doc.oggetto.descrizione;
            docPrincipaleNode.SelectSingleNode("StrutturaOriginale/TipoStruttura").InnerText = "DocumentoGenerico";
            docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idPr;
            docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/OrdinePresentazione").InnerText = "1";
            docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/TipoComponente").InnerText = "Contenuto";
            docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/TipoSupportoComponente").InnerText = "FILE";
            if (tipoDoc != TipologiaUnitaDocumentaria.FatturaElettronica)
            {
                docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/NomeComponente").InnerText = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, docPrincipale));
                docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/FormatoFileVersato").InnerText = this.getExtension(docPrincipale.fileName);
                docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/HashVersato").InnerText = this.getImpronta(docPrincipale);
                docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/IDComponenteVersato").InnerText = docPrincipale.versionId;

            }
            else
            {
                logger.Debug("Fattura Elettronica");
                if (!string.IsNullOrEmpty(docPrincipale.fileName))
                {
                    logger.Debug("File principale: " + docPrincipale.fileName);
                    //if (!docPrincipale.fileName.ToUpper().EndsWith("XML") && !docPrincipale.fileName.ToUpper().EndsWith("XML.P7M"))

                    // Controllo su conversioni pdf
                    if (docPrincipale.descrizione == "Documento convertito in pdf lato server")
                    {
                        logger.Debug("Doc principale convertito in PDF");
                   
                        int numVersioni = doc.documenti.Count;
                        logger.DebugFormat("{0} versioni", numVersioni);

                        if (numVersioni > 1)
                        {
                            DocsPaVO.documento.Documento previousVersion = (DocsPaVO.documento.Documento)doc.documenti[1];
                            if (previousVersion.fileName.ToUpper().EndsWith("XML") || previousVersion.fileName.ToUpper().EndsWith("XML.P7M"))
                            {
                                docPrincipale = (DocsPaVO.documento.Documento)doc.documenti[1];
                                logger.DebugFormat("Caricata versione {0} - nomefile {1}", docPrincipale.versionId, docPrincipale.fileName);
                            }
                        }

                    }

                    if(!CtrlXMLFattura(docPrincipale,infoUt,doc))
                    {
                        foreach (DocsPaVO.documento.Allegato allX in doc.allegati)
                        {
                            if (allX != null && !string.IsNullOrEmpty(allX.descrizione) 
                                //&& allX.descrizione.ToUpper() == "FATTURA"
                                && !string.IsNullOrEmpty(allX.fileName) && (allX.fileName.ToUpper().EndsWith("XML") || allX.fileName.ToUpper().EndsWith("XML.P7M")))
                            {
                                if (CtrlXMLFattura(allX, infoUt,doc))
                                {
                                    docScambiato.docNumber = docPrincipale.docNumber;
                                    docScambiato.versionId = docPrincipale.versionId;
                                    docScambiato.fileName = docPrincipale.fileName;
                                    //nomeFileRappr = docPrincipale.fileName;
                                    nomeFileRappr = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, docPrincipale));
                                    docScambiato.version = docPrincipale.version;
                                    docScambiato.versionLabel = docPrincipale.versionLabel;
                                    docScambiato.path = docPrincipale.path;
                                    docScambiato.fileSize = docPrincipale.fileSize;
                                    if (docPrincipale.repositoryContext != null)
                                        docScambiato.repositoryContext = docPrincipale.repositoryContext;

                                    docPrincipale.docNumber = allX.docNumber;
                                    docPrincipale.versionId = allX.versionId;
                                    docPrincipale.fileName = allX.fileName;
                                    docPrincipale.version = allX.version;
                                    docPrincipale.versionLabel = allX.versionLabel;
                                    docPrincipale.path = allX.path;

                                    //nomeFileFatt = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, allX));

                                    if (allX.repositoryContext != null)
                                        docPrincipale.repositoryContext = allX.repositoryContext;

                                    if (allX.descrizione.ToUpper() != "FATTURA")
                                        erroreDoppioScambio = true;

                                    docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/NomeComponente").InnerText = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, docPrincipale));
                                    docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/FormatoFileVersato").InnerText = this.getExtension(docPrincipale.fileName);
                                    docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/HashVersato").InnerText = this.getImpronta(docPrincipale);
                                    docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/IDComponenteVersato").InnerText = docPrincipale.versionId;
                                    scambioFattura = true;
                                    docPrincipaleNode.SelectSingleNode("IDDocumento").InnerText = allX.docNumber;

                                    //if(!nomeFileRappr.ToUpper().Contains(nomeFileFatt.ToUpper()))
                                    //    scambioFattura = false;


                                    logger.Debug("Scambio fattura");
                                    logger.Debug("Nome file rappr fattura: " + nomeFileRappr);
                                }
                            }
                        }
                    }
                    else
                    {
                        docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/NomeComponente").InnerText = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, docPrincipale));
                        docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/FormatoFileVersato").InnerText = this.getExtension(docPrincipale.fileName);
                        docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/HashVersato").InnerText = this.getImpronta(docPrincipale);
                        docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/IDComponenteVersato").InnerText = docPrincipale.versionId;
                        nomeFileFatt = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, docPrincipale));
                    }
                }
            }
            logger.Debug("docPrincipale filename e path");
            logger.Debug(docPrincipale.fileName);
            logger.Debug(docPrincipale.path);

            // inserisco nel dictionary
            //FileDocumento fileDocPrincipale = BusinessLogic.Documenti.FileManager.getfile(docPrincipale, infoUt);
            FileDocumento fileDocPrincipale = BusinessLogic.Documenti.FileManager.getFileFirmato(docPrincipale, infoUt, false);
            bool isTSD = false;
            if (fileDocPrincipale != null)
            {
                filesToSend.Add(idPr, fileDocPrincipale);

                // CHECK SE TSD
                logger.Debug("TSD check");
                if (!string.IsNullOrEmpty(fileDocPrincipale.nomeOriginale) && fileDocPrincipale.nomeOriginale.ToUpper().Trim().EndsWith("TSD"))
                {
                    logger.Debug("isTSD true");
                    isTSD = true;
                    if (!(docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/NomeComponente").InnerText.ToUpper().EndsWith("TSD")))
                        docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/NomeComponente").InnerText = docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/NomeComponente").InnerText + ".tsd";
                    if (!(docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/FormatoFileVersato").InnerText.ToUpper().EndsWith("TSD")))
                        docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/FormatoFileVersato").InnerText = docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente/FormatoFileVersato").InnerText + ".tsd";
                }

            }
            else
            {
                throw new Exception("CreateXMLDoc: errore nel reperimento del file (FileDocumento null)!");
            }
            if (tipoDoc == TipologiaUnitaDocumentaria.FatturaElettronica)
            {
                logger.Debug("filedocumento fullname name nomeoriginale path:");
                logger.Debug(fileDocPrincipale.fullName);
                logger.Debug(fileDocPrincipale.name);
                logger.Debug(fileDocPrincipale.nomeOriginale);
                logger.Debug(fileDocPrincipale.path);
                nomeFileFatt = fileDocPrincipale.nomeOriginale;
                if (!string.IsNullOrEmpty(nomeFileFatt))
                {
                    if (!nomeFileRappr.ToUpper().Contains(nomeFileFatt.ToUpper()))
                    {
                        erroreScambio = true;
                        erroreDoppioScambio = false;
                    }
                }
            }
            // verifico l'eventuale presenza di marche (SE NON E' TSD)
            if (!(BusinessLogic.Documenti.FileManager.getExtFileFromPath(fileDocPrincipale.name).ToUpper().Contains("TSD")) && !isTSD)
            {
                ArrayList timestampArray = TimestampManager.getTimestampsDoc(infoUt, docPrincipale);
                if (timestampArray != null && timestampArray.Count > 0)
                {
                    XmlElement sottoComp = xml.CreateElement("SottoComponenti");

                    // contatore sottocomponenti associate al componente
                    int sottocomponenti = 1;

                    // aggiungo sottocomponente marca
                    foreach (DocsPaVO.documento.TimestampDoc ts in timestampArray)
                    {
                        XmlElement sc = this.AddSottoComponente(ref xml, docPrincipale, ts);
                        string idCompTSR = doc.docNumber + "_C" + componenti.ToString() + "_SC" + sottocomponenti.ToString();
                        sc.SelectSingleNode("ID").InnerText = idCompTSR;
                        sc.SelectSingleNode("OrdinePresentazione").InnerText = (sottocomponenti + 1).ToString();

                        // costruisco il filedocumento da inserire nel dictionary
                        FileDocumento fdTSR = new FileDocumento();
                        fdTSR.estensioneFile = "TSR";
                        fdTSR.nomeOriginale = ts.DOC_NUMBER + "_" + ts.NUM_SERIE + ".tsr";
                        fdTSR.fullName = ts.DOC_NUMBER + "_" + ts.NUM_SERIE + ".tsr";
                        fdTSR.content = Encoding.UTF8.GetBytes(ts.TSR_FILE);
                        fdTSR.length = fdTSR.content.Length;
                        filesToSend.Add(idCompTSR, fdTSR);

                        sottocomponenti++;
                        sottoComp.AppendChild(sc);

                    }

                    docPrincipaleNode.SelectSingleNode("StrutturaOriginale/Componenti/Componente").AppendChild(sottoComp);
                }

            }

            #endregion

            // incremento il contatore
            componenti++;

            #region Allegati, Annessi, Annotazioni

            // Allegati utente
            if (tipoDoc != TipologiaUnitaDocumentaria.FatturaElettronica)
            {
                logger.Debug("Allegati utente");
                ArrayList allegatiUtente = BusinessLogic.Documenti.AllegatiManager.getAllegati(doc.docNumber, "user");
                ArrayList annotazioni = new ArrayList();
                if (allegatiUtente != null && allegatiUtente.Count > 0)
                {

                    int numAllegati = allegatiUtente.Count;
                    logger.Debug(string.Format("{0} allegati", numAllegati));

                    foreach (DocsPaVO.documento.Allegato all in allegatiUtente)
                    {

                        if (!(all.TypeAttachment.Equals(2) || all.TypeAttachment.Equals(3)))
                        {
                            // identifico le segnature XML da cha_interop e oggetto
                            if (!(doc.tipoProto.Equals("A") && !string.IsNullOrEmpty(doc.interop) && doc.interop.Equals("S") && all.descrizione.ToUpper().Contains("ALLEGATO SEGNATURA")))
                            {
                                string idAll = doc.docNumber + "_C" + componenti.ToString();

                                XmlElement element = this.AddMetadatiElement(ref xml, all, "1", infoUt);
                                element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAll;

                                // inserisco il file nel dictionary
                                FileDocumento fdAll = new FileDocumento();
                                if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                                {
                                    //fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUt);
                                    fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                                    if (fdAll != null)
                                        filesToSend.Add(idAll, fdAll);
                                }


                                if (element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/SottoComponenti") != null)
                                {
                                    XmlNodeList sc = element.SelectNodes("StrutturaOriginale/Componenti/Componente/SottoComponenti/SottoComponente");
                                    int sottocomponenti = 1;
                                    foreach (XmlNode n in sc)
                                    {
                                        string idSc = doc.docNumber + "_C" + componenti.ToString() + "_SC" + sottocomponenti.ToString();
                                        n.SelectSingleNode("ID").InnerText = idSc;
                                        n.SelectSingleNode("OrdinePresentazione").InnerText = sottocomponenti.ToString();

                                        sottocomponenti++;
                                    }
                                }
                                componenti++;
                                xml.SelectSingleNode("UnitaDocumentaria/Allegati").AppendChild(element);
                            }
                            else
                            {
                                numAllegati = numAllegati - 1;
                                annotazioni.Add(all);
                            }
                        }
                        else
                        {
                            numAllegati = numAllegati - 1;
                        }
                    }

                    // se tutti gli allegati utente sono in realtà segnature xml devo rimuovere il blocco
                    if (numAllegati > 0)
                        xml.SelectSingleNode("UnitaDocumentaria/NumeroAllegati").InnerText = numAllegati.ToString();
                    else
                    {
                        XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                        node.RemoveChild(node.SelectSingleNode("NumeroAllegati"));
                        node.RemoveChild(node.SelectSingleNode("Allegati"));
                    }
                }
                else
                {
                    XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                    node.RemoveChild(node.SelectSingleNode("NumeroAllegati"));
                    node.RemoveChild(node.SelectSingleNode("Allegati"));
                }
                // Il primo annesso è il file XML con i metadati aggiuntivi dei documenti
                XmlElement metadatiElement = this.AddMetadatiElement(ref xml, docPrincipale, "M", infoUt);
                string idAnnMetadati = doc.docNumber + "_C" + componenti.ToString();
                metadatiElement.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAnnMetadati;

                // Creo il FileDocumento e ci inserisco il file XML da DB
                FileDocumento fdMeta = new FileDocumento();
                fdMeta.estensioneFile = "XML";
                fdMeta.nomeOriginale = "Metadati_PITre.xml";
                fdMeta.fullName = "Metadati_PITre.xml";
                fdMeta.content = Encoding.UTF8.GetBytes(this.getFileMetadati(doc.docNumber));
                fdMeta.length = fdMeta.content.Length;
                filesToSend.Add(idAnnMetadati, fdMeta);

                metadatiElement.SelectSingleNode("StrutturaOriginale/Componenti/Componente/HashVersato").InnerText = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fdMeta.content);

                componenti++;
                xml.SelectSingleNode("UnitaDocumentaria/Annessi").AppendChild(metadatiElement);

                // Allegati PEC, PITre e Sistemi Esterni
                ArrayList annessi = new ArrayList();
                foreach (DocsPaVO.documento.Allegato all in doc.allegati)
                {
                    if ((all.TypeAttachment.Equals(2)) || (all.TypeAttachment.Equals(3)) || (all.TypeAttachment.Equals(4)))
                        annessi.Add(all);
                }

                if (annessi != null && annessi.Count > 0)
                {
                    foreach (DocsPaVO.documento.Allegato all in annessi)
                    {
                        XmlElement element = this.AddMetadatiElement(ref xml, all, all.TypeAttachment.ToString(), infoUt);
                        string idAll = doc.docNumber + "_C" + componenti.ToString();
                        element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAll;

                        // inserisco il file nel dictionary
                        FileDocumento fdAll = new FileDocumento();
                        if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                        {
                            //fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUt);
                            fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                            if (fdAll != null)
                                filesToSend.Add(idAll, fdAll);
                        }

                        if (element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/SottoComponenti") != null)
                        {
                            XmlNodeList sc = element.SelectNodes("StrutturaOriginale/Componenti/Componente/SottoComponenti/SottoComponente");
                            int sottocomponenti = 1;
                            foreach (XmlNode n in sc)
                            {
                                n.SelectSingleNode("ID").InnerText = doc.docNumber + "_C" + componenti.ToString() + "_SC" + sottocomponenti.ToString();
                                n.SelectSingleNode("OrdinePresentazione").InnerText = sottocomponenti.ToString();
                                sottocomponenti++;
                            }
                        }
                        componenti++;
                        xml.SelectSingleNode("UnitaDocumentaria/Annessi").AppendChild(element);
                    }

                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAnnessi").InnerText = (annessi.Count + 1).ToString();
                }
                else
                {
                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAnnessi").InnerText = "1";
                }

                // Annotazioni
                if (annotazioni != null && annotazioni.Count > 0)
                {
                    foreach (DocsPaVO.documento.Allegato all in annotazioni)
                    {
                        XmlElement element = this.AddMetadatiElement(ref xml, all, "S", infoUt);
                        string idAll = doc.docNumber + "_C" + componenti.ToString();
                        element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAll;

                        FileDocumento fdAll = new FileDocumento();
                        if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                        {
                            //fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUt);
                            fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                            if (fdAll != null)
                                filesToSend.Add(idAll, fdAll);
                        }

                        componenti++;
                        xml.SelectSingleNode("UnitaDocumentaria/Annotazioni").AppendChild(element);
                    }

                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAnnotazioni").InnerText = annotazioni.Count.ToString();
                }
                else
                {
                    XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                    node.RemoveChild(node.SelectSingleNode("NumeroAnnotazioni"));
                    node.RemoveChild(node.SelectSingleNode("Annotazioni"));
                }

            }
            #endregion

            #region allegati, annessi e annotazioni per fattura elettronica.
            // Allegati utente
            if (tipoDoc == TipologiaUnitaDocumentaria.FatturaElettronica)
            {
                logger.Debug("Allegati utente");
                ArrayList allegatiUtente = BusinessLogic.Documenti.AllegatiManager.getAllegati(doc.docNumber, "user");
                ArrayList annotazioni = new ArrayList();
                ArrayList annessi = new ArrayList();
                int numAnnessiScambia = 0;
                int numAllegati = 0;

                if (allegatiUtente != null && allegatiUtente.Count > 0)
                {

                    numAllegati = allegatiUtente.Count;
                    logger.Debug(string.Format("{0} allegati", numAllegati));

                    foreach (DocsPaVO.documento.Allegato all in allegatiUtente)
                    {
                        // identifico le segnature XML da cha_interop e oggetto
                        if (!(doc.tipoProto.Equals("A") && !string.IsNullOrEmpty(doc.interop) && doc.interop.Equals("S") && all.descrizione.ToUpper().Contains("ALLEGATO SEGNATURA")))
                        {
                            if (!string.IsNullOrEmpty(all.descrizione)
                                && !all.descrizione.Replace(" ", string.Empty).ToUpper().Equals("NOTIFICADECORRENZATERMINI")
                                && !all.descrizione.Replace(" ", string.Empty).ToUpper().Equals("NOTIFICAESITOCOMMITTENTE")
                                //&& !all.descrizione.Replace(" ", string.Empty).ToUpper().Equals("FATTURA")
                                //&& !all.fileName.ToUpper().Contains(nomeFileFatt.ToUpper())
                                )
                            {
                                nomeAllegato = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, all));
                                logger.Debug("nome allegato: " + nomeAllegato);
                                XmlElement element = null;
                                if (erroreScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper() + ".PDF"))
                                {
                                    logger.Debug("Annesso Rappresentazione Fattura. Errore Doppio Scambio. Filename " + nomeAllegato);
                                    element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                                }
                                else if (erroreScambio && scambioFattura && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                                {
                                    all.docNumber = docScambiato.docNumber;
                                    all.versionId = docScambiato.versionId;
                                    all.fileName = docScambiato.fileName;
                                    all.version = docScambiato.version;
                                    all.versionLabel = docScambiato.versionLabel;
                                    all.path = docScambiato.path;
                                    all.fileSize = docScambiato.fileSize;
                                    logger.Debug("Allegato alla fattura. Scambio fattura TRUE. Filename " + all.fileName);

                                    if (docScambiato.repositoryContext != null)
                                        all.repositoryContext = ((DocsPaVO.documento.Documento)doc.documenti[0]).repositoryContext;
                                    element = this.AddMetadatiElement(ref xml, all, "1", infoUt);
                                }
                                else if (erroreDoppioScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                                {
                                    all.docNumber = docScambiato.docNumber;
                                    all.versionId = docScambiato.versionId;
                                    all.fileName = docScambiato.fileName;
                                    all.version = docScambiato.version;
                                    all.versionLabel = docScambiato.versionLabel;
                                    all.path = docScambiato.path;
                                    all.fileSize = docScambiato.fileSize;
                                    logger.Debug("Allegato alla fattura. Doppio Scambio. Filename " + all.fileName);

                                    if (docScambiato.repositoryContext != null)
                                        all.repositoryContext = ((DocsPaVO.documento.Documento)doc.documenti[0]).repositoryContext;
                                    element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                                }
                                else                                
                                {
                                    element = this.AddMetadatiElement(ref xml, all, "1", infoUt);
                                }

                                string idAll = doc.docNumber + "_C" + componenti.ToString();

                                
                                element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAll;

                                // inserisco il file nel dictionary
                                FileDocumento fdAll = new FileDocumento();
                                if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                                {
                                    if (erroreScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper() + ".PDF"))
                                    {
                                        fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                                    }
                                    else if (scambioFattura && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                                    {
                                        logger.Debug("Prelevo doc scambiato");
                                        fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(docScambiato, infoUt, false);
                                    }
                                    else if (erroreDoppioScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                                    {
                                        fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(docScambiato, infoUt, false);
                                    }
                                    else
                                        //fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUt);
                                        fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                                    if (fdAll != null)
                                        filesToSend.Add(idAll, fdAll);
                                }


                                if (element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/SottoComponenti") != null)
                                {
                                    XmlNodeList sc = element.SelectNodes("StrutturaOriginale/Componenti/Componente/SottoComponenti/SottoComponente");
                                    int sottocomponenti = 1;
                                    foreach (XmlNode n in sc)
                                    {
                                        string idSc = doc.docNumber + "_C" + componenti.ToString() + "_SC" + sottocomponenti.ToString();
                                        n.SelectSingleNode("ID").InnerText = idSc;
                                        n.SelectSingleNode("OrdinePresentazione").InnerText = sottocomponenti.ToString();

                                        sottocomponenti++;
                                    }
                                }
                                componenti++;
                                // FIX PER ANNESSI IN TAG ALLEGATI IN CASO DI DOPPIO SCAMBIO
                                //(if (erroreDoppioScambio)
                                if ((erroreDoppioScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper())) || (erroreScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper() + ".PDF")))
                                {
                                    xml.SelectSingleNode("UnitaDocumentaria/Annessi").AppendChild(element);
                                    numAllegati = numAllegati - 1;
                                    numAnnessiScambia = numAnnessiScambia + 1;
                                }
                                else
                                {
                                    xml.SelectSingleNode("UnitaDocumentaria/Allegati").AppendChild(element);
                                }
                            }
                            else
                            {
                                numAllegati = numAllegati - 1;
                                annessi.Add(all);
                            }
                        }
                        else
                        {
                            numAllegati = numAllegati - 1;
                            annotazioni.Add(all);
                        }
                    }

                    // se tutti gli allegati utente sono in realtà segnature xml devo rimuovere il blocco
                    if (numAllegati > 0)
                        xml.SelectSingleNode("UnitaDocumentaria/NumeroAllegati").InnerText = numAllegati.ToString();
                    else
                    {
                        // Commento, verrà fatto dopo
                        //XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                        //node.RemoveChild(node.SelectSingleNode("NumeroAllegati"));
                        //node.RemoveChild(node.SelectSingleNode("Allegati"));
                    }
                }
                else
                {
                    // Commento, verrà fatto dopo
                    //XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                    //node.RemoveChild(node.SelectSingleNode("NumeroAllegati"));
                    //node.RemoveChild(node.SelectSingleNode("Allegati"));
                }
                // Il primo annesso è il file XML con i metadati aggiuntivi dei documenti
                XmlElement metadatiElement = this.AddMetadatiElement(ref xml, docPrincipale, "M", infoUt);
                string idAnnMetadati = doc.docNumber + "_C" + componenti.ToString();
                metadatiElement.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAnnMetadati;

                // Creo il FileDocumento e ci inserisco il file XML da DB
                FileDocumento fdMeta = new FileDocumento();
                fdMeta.estensioneFile = "XML";
                fdMeta.nomeOriginale = "Metadati_PITre.xml";
                fdMeta.fullName = "Metadati_PITre.xml";
                fdMeta.content = Encoding.UTF8.GetBytes(this.getFileMetadati(doc.docNumber));
                fdMeta.length = fdMeta.content.Length;
                filesToSend.Add(idAnnMetadati, fdMeta);

                metadatiElement.SelectSingleNode("StrutturaOriginale/Componenti/Componente/HashVersato").InnerText = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fdMeta.content);

                componenti++;
                xml.SelectSingleNode("UnitaDocumentaria/Annessi").AppendChild(metadatiElement);

                // Allegati PEC, PITre e Sistemi Esterni
                foreach (DocsPaVO.documento.Allegato all in doc.allegati)
                {
                    if ((all.TypeAttachment.Equals(2)) || (all.TypeAttachment.Equals(3)) || (all.TypeAttachment.Equals(4)))
                        annessi.Add(all);
                }

                if (annessi != null && annessi.Count > 0)
                {
                    foreach (DocsPaVO.documento.Allegato all in annessi)
                    {
                        XmlElement element = null;
                        if (!string.IsNullOrEmpty(all.descrizione)
                                && !all.descrizione.Replace(" ", string.Empty).ToUpper().Equals("NOTIFICADECORRENZATERMINI")
                                && !all.descrizione.Replace(" ", string.Empty).ToUpper().Equals("NOTIFICAESITOCOMMITTENTE")
                            //&& !all.descrizione.Replace(" ", string.Empty).ToUpper().Equals("FATTURA")
                                && !all.fileName.ToUpper().Contains(nomeFileFatt.ToUpper())
                                )
                        {
                            element = this.AddMetadatiElement(ref xml, all, all.TypeAttachment.ToString(), infoUt);
                        }
                        else
                        {
                            switch (all.descrizione.Replace(" ", string.Empty).ToUpper())
                            {
                                case "NOTIFICADECORRENZATERMINI":
                                    element = this.AddMetadatiElement(ref xml, all, "FNDT", infoUt);

                                    break;
                                case "NOTIFICAESITOCOMMITTENTE":
                                    element = this.AddMetadatiElement(ref xml, all, "FNEC", infoUt);

                                    break;
                                //case "FATTURA":
                                //    if (!scambioFattura)
                                //    {
                                //        logger.Debug("Annesso Rappresentazione Fattura. Scambio fattura FALSE. Filename " + all.fileName);
                                //        element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                                //    }
                                //    else
                                //    {
                                //        all.docNumber = docScambiato.docNumber;
                                //        all.versionId = docScambiato.versionId;
                                //        all.fileName = docScambiato.fileName;
                                //        all.version = docScambiato.version;
                                //        all.versionLabel = docScambiato.versionLabel;
                                //        all.path = docScambiato.path;
                                //        all.fileSize = docScambiato.fileSize;
                                //        logger.Debug("Annesso Rappresentazione Fattura. Scambio fattura TRUE. Filename " + all.fileName);

                                //        if (docScambiato.repositoryContext != null)
                                //            all.repositoryContext = ((DocsPaVO.documento.Documento)doc.documenti[0]).repositoryContext;
                                //        element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                                //    }
                                //    break;
                            }
                        }
                        nomeAllegato = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(infoUt, all));
                        logger.Debug("Nome annesso: "+nomeAllegato);
                        if (!scambioFattura && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                        {
                            logger.Debug("Annesso Rappresentazione Fattura. Scambio fattura FALSE. Filename " + nomeAllegato);
                            element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                        }
                        else if (erroreScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()+".PDF"))
                        {
                            logger.Debug("Annesso Rappresentazione Fattura. Errore Scambio. Filename " + nomeAllegato);
                            element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                        }
                        else if (scambioFattura && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                        {
                            all.docNumber = docScambiato.docNumber;
                            all.versionId = docScambiato.versionId;
                            all.fileName = docScambiato.fileName;
                            all.version = docScambiato.version;
                            all.versionLabel = docScambiato.versionLabel;
                            all.path = docScambiato.path;
                            all.fileSize = docScambiato.fileSize;
                            
                            logger.Debug("Annesso Rappresentazione Fattura. Scambio fattura TRUE. Filename " + nomeAllegato);

                            if (docScambiato.repositoryContext != null)
                                all.repositoryContext = ((DocsPaVO.documento.Documento)doc.documenti[0]).repositoryContext;
                            element = this.AddMetadatiElement(ref xml, all, "FRF", infoUt);
                        }
                        else if (erroreDoppioScambio && all.descrizione.ToUpper() == "FATTURA")
                        {
                            element = this.AddMetadatiElement(ref xml, all, "1", infoUt);
                        }

                        string idAll = doc.docNumber + "_C" + componenti.ToString();
                        element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAll;

                        // inserisco il file nel dictionary
                        FileDocumento fdAll = new FileDocumento();
                        if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                        {
                            //fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUt);
                            //if (scambioFattura && all.descrizione.Replace(" ", string.Empty).ToUpper() == "FATTURA")
                            if (erroreScambio && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper() + ".PDF"))
                            {
                                fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                            }
                            else if (scambioFattura && nomeAllegato.ToUpper().Contains(nomeFileFatt.ToUpper()))
                            {
                                logger.Debug("Prelevo doc scambiato");
                                fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(docScambiato, infoUt, false);
                            }
                            else
                                fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);

                            logger.Debug("FileDocumento FdAll: " + fdAll.fullName);

                            if (fdAll != null)
                                filesToSend.Add(idAll, fdAll);
                        }

                        if (element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/SottoComponenti") != null)
                        {
                            XmlNodeList sc = element.SelectNodes("StrutturaOriginale/Componenti/Componente/SottoComponenti/SottoComponente");
                            int sottocomponenti = 1;
                            foreach (XmlNode n in sc)
                            {
                                n.SelectSingleNode("ID").InnerText = doc.docNumber + "_C" + componenti.ToString() + "_SC" + sottocomponenti.ToString();
                                n.SelectSingleNode("OrdinePresentazione").InnerText = sottocomponenti.ToString();
                                sottocomponenti++;
                            }
                        }
                        componenti++;
                        if (erroreDoppioScambio && all.descrizione.ToUpper() == "FATTURA")
                        {
                            xml.SelectSingleNode("UnitaDocumentaria/Allegati").AppendChild(element);
                            numAllegati = numAllegati + 1;
                            numAnnessiScambia = numAnnessiScambia - 1;
                        }
                        else
                        {
                            xml.SelectSingleNode("UnitaDocumentaria/Annessi").AppendChild(element);
                        }
                    }

                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAnnessi").InnerText = (annessi.Count + 1 + numAnnessiScambia).ToString();
                }
                else
                {
                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAnnessi").InnerText = (1 + numAnnessiScambia).ToString();
                }

                // Ora verifico se rimuovere il campo allegati
                if (numAllegati > 0)
                {
                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAllegati").InnerText = numAllegati.ToString();
                }
                else
                {
                    XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                    node.RemoveChild(node.SelectSingleNode("NumeroAllegati"));
                    node.RemoveChild(node.SelectSingleNode("Allegati"));
                }

                // Annotazioni
                if (annotazioni != null && annotazioni.Count > 0)
                {
                    foreach (DocsPaVO.documento.Allegato all in annotazioni)
                    {
                        XmlElement element = this.AddMetadatiElement(ref xml, all, "S", infoUt);
                        string idAll = doc.docNumber + "_C" + componenti.ToString();
                        element.SelectSingleNode("StrutturaOriginale/Componenti/Componente/ID").InnerText = idAll;

                        FileDocumento fdAll = new FileDocumento();
                        if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                        {
                            //fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUt);
                            fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUt, false);
                            if (fdAll != null)
                                filesToSend.Add(idAll, fdAll);
                        }

                        componenti++;
                        xml.SelectSingleNode("UnitaDocumentaria/Annotazioni").AppendChild(element);
                    }

                    xml.SelectSingleNode("UnitaDocumentaria/NumeroAnnotazioni").InnerText = annotazioni.Count.ToString();
                }
                else
                {
                    XmlNode node = xml.SelectSingleNode("UnitaDocumentaria");
                    node.RemoveChild(node.SelectSingleNode("NumeroAnnotazioni"));
                    node.RemoveChild(node.SelectSingleNode("Annotazioni"));
                }

            }
            #endregion
            // Campi specifici
            this.AddDatiSpecificiDoc(doc, ref xml, tipoDoc, amm, utenteCreatore, ruoloCreatore, infoUt);

            logger.Debug("END");

            return xml;
        }

        private XmlElement AddSottoFascicoloElement(ref XmlDocument xml, Fascicolo fascicolo, Folder folder)
        {

            logger.Debug("BEGIN");
            ArrayList list = this.getFolderTree(folder);
            XmlElement sottoFascicolo = xml.CreateElement("SottoFascicolo");
            XmlElement indentificativo = xml.CreateElement("Identificativo");
            XmlElement oggetto = xml.CreateElement("Oggetto");

            string tree = fascicolo.codice;
            foreach (Folder item in list)
            {
                if (!string.IsNullOrEmpty(tree))
                    tree = tree + "/";
                tree = tree + item.descrizione;
            }

            indentificativo.InnerText = tree;
            oggetto.InnerText = folder.descrizione;

            sottoFascicolo.AppendChild(indentificativo);
            sottoFascicolo.AppendChild(oggetto);

            logger.Debug("END");

            return sottoFascicolo;
        }

        private XmlElement AddMetadatiElement(ref XmlDocument xml, DocsPaVO.documento.FileRequest doc, string code, InfoUtente utente)
        {

            string name = string.Empty;
            string tipo = string.Empty;

            switch (code)
            {
                case "1":
                    name = "Allegato";
                    tipo = "Allegato utente";
                    // se l'allegato non è acquisito...
                    if (!(!string.IsNullOrEmpty(doc.fileSize) && Convert.ToInt32(doc.fileSize) > 0))
                        tipo = "Allegato utente - non acquisito";
                    break;

                case "2":
                    name = "Annesso";
                    tipo = "Allegato PEC";
                    break;

                case "3":
                    name = "Annesso";
                    tipo = "Allegato Pitre";
                    break;

                case "4":
                    name = "Annesso";
                    tipo = "Altri sistemi";
                    break;

                case "M":
                    name = "Annesso";
                    tipo = "Metadati PITre";
                    break;

                case "S":
                    name = "Annotazione";
                    tipo = "Segnatura xml";
                    break;

                case "FNEC":
                    name = "Annesso";
                    tipo = "Notifica di esito committente";
                    break;

                case "FNDT":
                    name = "Annesso";
                    tipo = "Notifica di decorrenza dei termini";
                    break;

                case "FRF":
                    name = "Annesso";
                    tipo = "Rappresentazione Fattura";
                    break;
            }


            XmlElement element = xml.CreateElement(name);

            if (!(tipo.Equals("Documento Principale") || tipo.Equals("Metadati PITre")))
            {
                DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)doc;

                XmlElement idDoc = xml.CreateElement("IDDocumento");
                XmlElement tipoDoc = xml.CreateElement("TipoDocumento");
                XmlElement profilo = xml.CreateElement("ProfiloDocumento");
                XmlElement descrizione = xml.CreateElement("Descrizione");
                XmlElement struttura = xml.CreateElement("StrutturaOriginale");
                XmlElement tipoStruttura = xml.CreateElement("TipoStruttura");
                XmlElement componenti = xml.CreateElement("Componenti");

                idDoc.InnerText = all.docNumber;
                tipoDoc.InnerText = tipo;
                descrizione.InnerText = all.descrizione;
                tipoStruttura.InnerText = "DocumentoGenerico";

                componenti.AppendChild(this.AddComponente(ref xml, all, "Contenuto", utente));

                profilo.AppendChild(descrizione);
                struttura.AppendChild(tipoStruttura);
                struttura.AppendChild(componenti);

                element.AppendChild(idDoc);
                element.AppendChild(tipoDoc);
                element.AppendChild(profilo);
                element.AppendChild(struttura);
            }
            if (tipo.Equals("Metadati PITre"))
            {
                XmlElement idDoc = xml.CreateElement("IDDocumento");
                XmlElement tipoDoc = xml.CreateElement("TipoDocumento");
                XmlElement profilo = xml.CreateElement("ProfiloDocumento");
                XmlElement descrizione = xml.CreateElement("Descrizione");
                XmlElement struttura = xml.CreateElement("StrutturaOriginale");
                XmlElement tipoStruttura = xml.CreateElement("TipoStruttura");

                XmlElement componenti = xml.CreateElement("Componenti");
                XmlElement componente = xml.CreateElement("Componente");
                XmlElement id = xml.CreateElement("ID"); // valorizzato fuori
                XmlElement ordine = xml.CreateElement("OrdinePresentazione");
                XmlElement tipoComponente = xml.CreateElement("TipoComponente");
                XmlElement tipoSupporto = xml.CreateElement("TipoSupportoComponente");
                XmlElement nome = xml.CreateElement("NomeComponente");
                XmlElement formato = xml.CreateElement("FormatoFileVersato");
                XmlElement hash = xml.CreateElement("HashVersato");
                XmlElement idVersione = xml.CreateElement("IDComponenteVersato");
                //XmlElement rifTermporale = xml.CreateElement("RiferimentoTemporale");
                //XmlElement descrizioneRifTemporale = xml.CreateElement("DescrizioneRiferimentoTemporale");

                idDoc.InnerText = doc.docNumber + "_META"; // ID??
                tipoDoc.InnerText = tipo;
                descrizione.InnerText = "Dati del documento in PITre";
                tipoStruttura.InnerText = "DocumentoGenerico";

                ordine.InnerText = "1";
                tipoComponente.InnerText = "Contenuto";
                tipoSupporto.InnerText = "FILE";
                nome.InnerText = "Metadati_PITre.xml";
                formato.InnerText = "xml";
                hash.InnerText = string.Empty; //impronta??
                idVersione.InnerText = idDoc.InnerText;
                // riferimento temporale

                profilo.AppendChild(descrizione);

                componente.AppendChild(id);
                componente.AppendChild(ordine);
                componente.AppendChild(tipoComponente);
                componente.AppendChild(tipoSupporto);
                componente.AppendChild(nome);
                componente.AppendChild(formato);
                componente.AppendChild(hash);
                componente.AppendChild(idVersione);
                //componente.AppendChild(rifTermporale);
                //componente.AppendChild(descrizioneRifTemporale);

                componenti.AppendChild(componente);

                struttura.AppendChild(tipoStruttura);
                struttura.AppendChild(componenti);

                element.AppendChild(idDoc);
                element.AppendChild(tipoDoc);
                element.AppendChild(profilo);
                element.AppendChild(struttura);
            }

            return element;

        }

        private XmlElement AddComponente(ref XmlDocument xml, DocsPaVO.documento.FileRequest doc, string tipoComp, InfoUtente utente)
        {
            XmlElement element = xml.CreateElement("Componente");

            DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
            string impronta = string.Empty;

            // verifico se il documento è stato acqusito
            bool isAcquired = false;
            if (!string.IsNullOrEmpty(doc.fileSize) && Convert.ToInt32(doc.fileSize) > 0)
                isAcquired = true;

            XmlElement id = xml.CreateElement("ID"); // valorizzato fuori
            XmlElement ordine = xml.CreateElement("OrdinePresentazione");
            XmlElement tipoComponente = xml.CreateElement("TipoComponente");
            XmlElement tipoSupporto = xml.CreateElement("TipoSupportoComponente");
            XmlElement nome = xml.CreateElement("NomeComponente");
            XmlElement formato = xml.CreateElement("FormatoFileVersato");
            XmlElement hash = xml.CreateElement("HashVersato");
            XmlElement idVersione = xml.CreateElement("IDComponenteVersato");
            XmlElement rifTermporale = xml.CreateElement("RiferimentoTemporale");
            XmlElement descrizioneRifTemporale = xml.CreateElement("DescrizioneRiferimentoTemporale");

            #region Riferimento temporale

            bool addRifTemporale = false;
            if (doc.firmato.Equals("1"))
            {
                SchedaDocumento sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, doc.docNumber);
                string descrizioneRif = string.Empty;
                string dataRif = this.getRiferimentoTemporale(sch, utente, out descrizioneRif);
                if (!(string.IsNullOrEmpty(dataRif) && string.IsNullOrEmpty(descrizioneRif)))
                {
                    rifTermporale.InnerText = dataRif;
                    descrizioneRifTemporale.InnerText = descrizioneRif;
                    addRifTemporale = true;
                }
            }


            if (TimestampManager.getCountTimestampsDoc(utente, doc) > 0)
            {
                //addRifTemporale = true;
                ArrayList timestamps = TimestampManager.getTimestampsDoc(utente, doc);
                //DocsPaVO.documento.TimestampDoc ts = (DocsPaVO.documento.TimestampDoc)TimestampManager.getTimestampsDoc(utente, doc)[0];

                //rifTermporale.InnerText = this.formatDateRifTemp(((DocsPaVO.documento.TimestampDoc)timestamps[0]).DTA_CREAZIONE);
                //descrizioneRifTemporale.InnerText = "Marca temporale";

                if (!BusinessLogic.Documenti.FileManager.getExtFileFromPath(doc.fileName).ToUpper().Contains("TSD"))
                {
                    XmlElement sottocomponenti = xml.CreateElement("SottoComponenti");
                    foreach (DocsPaVO.documento.TimestampDoc tsr in timestamps)
                    {
                        XmlElement sc = this.AddSottoComponente(ref xml, doc, tsr);
                        sottocomponenti.AppendChild(sc);
                    }
                }
            }
            /*
            else
            {
                // scheda doc
                SchedaDocumento scheda = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, doc.docNumber);
                TipologiaUnitaDocumentaria tipo = this.getTipoDocumento(scheda, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione).Codice);
                if (tipo.Equals(TipologiaUnitaDocumentaria.DocProtocollato))
                {
                    addRifTemporale = true;
                    rifTermporale.InnerText = this.formatDateRifTemp(scheda.protocollo.dataProtocollazione);
                    descrizioneRifTemporale.InnerText = "Data di protocollazione";
                }
                else if (tipo.Equals(TipologiaUnitaDocumentaria.DocRepertoriato))
                {
                    addRifTemporale = true;
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = this.getContatoreRepertorio(scheda);
                    rifTermporale.InnerText = this.formatDateRifTemp(contatore.DATA_INSERIMENTO);
                    descrizioneRifTemporale.InnerText = "Data di repertoriazione";
                }
            }
            */
            #endregion

            ordine.InnerText = "1";
            tipoComponente.InnerText = tipoComp;
            idVersione.InnerText = doc.versionId;

            if (isAcquired)
            {
                tipoSupporto.InnerText = "FILE";
                nome.InnerText = BusinessLogic.Documenti.FileManager.getFileNameFromPath(this.GetNomeOriginale(utente, doc));
                formato.InnerText = this.getExtension(doc.fileName);
                docs.GetImpronta(out impronta, doc.versionId, doc.docNumber);
                hash.InnerText = impronta;
            }
            else
            {
                tipoSupporto.InnerText = "METADATI";
            }

            element.AppendChild(id);
            element.AppendChild(ordine);
            element.AppendChild(tipoComponente);
            element.AppendChild(tipoSupporto);
            if (isAcquired)
            {
                element.AppendChild(nome);
                element.AppendChild(formato);
                element.AppendChild(hash);
                element.AppendChild(idVersione);
            }
            if (addRifTemporale)
            {
                element.AppendChild(rifTermporale);
                element.AppendChild(descrizioneRifTemporale);
            }

            return element;

        }

        private XmlElement AddSottoComponente(ref XmlDocument xml, DocsPaVO.documento.FileRequest doc, DocsPaVO.documento.TimestampDoc ts)
        {
            XmlElement element = xml.CreateElement("SottoComponente");

            XmlElement id = xml.CreateElement("ID"); // valorizzato fuori
            XmlElement ordine = xml.CreateElement("OrdinePresentazione"); // valorizzato fuori
            XmlElement tipoComponente = xml.CreateElement("TipoComponente");
            XmlElement tipoSupporto = xml.CreateElement("TipoSupportoComponente");
            XmlElement nome = xml.CreateElement("NomeComponente");
            XmlElement formato = xml.CreateElement("FormatoFileVersato");
            XmlElement idVersione = xml.CreateElement("IDComponenteVersato");

            tipoComponente.InnerText = "Marca";
            tipoSupporto.InnerText = "FILE";
            nome.InnerText = ts.DOC_NUMBER + "_" + ts.NUM_SERIE + ".tsr";
            formato.InnerText = "TSR";
            idVersione.InnerText = doc.versionId + "_" + ts.NUM_SERIE;

            element.AppendChild(id);
            element.AppendChild(ordine);
            element.AppendChild(tipoComponente);
            element.AppendChild(tipoSupporto);
            element.AppendChild(nome);
            element.AppendChild(formato);
            element.AppendChild(idVersione);

            return element;
        }

        private void AddDatiSpecificiDoc(SchedaDocumento doc, ref XmlDocument xmlDoc, TipologiaUnitaDocumentaria tipo, DocsPaVO.amministrazione.InfoAmministrazione amm, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, InfoUtente infoUt)
        {

            logger.Debug(string.Format("BEGIN - ID={0}", doc.systemId));

            XmlNode nodeIntestazione = xmlDoc.SelectSingleNode("UnitaDocumentaria/Intestazione");
            XmlNode nodeUD = xmlDoc.SelectSingleNode("UnitaDocumentaria/ProfiloUnitaDocumentaria");
            XmlNode nodeDatiSpecifici = xmlDoc.SelectSingleNode("UnitaDocumentaria/DatiSpecifici");

            /*
            int numMarche = TimestampManager.getCountTimestampsDoc(infoUt, (DocsPaVO.documento.Documento)doc.documenti[0]);

            if (numMarche > 0)
            {
                // estraggo la marca più recente (sono ordinate per data creazione desc)
                DocsPaVO.documento.TimestampDoc ts = (DocsPaVO.documento.TimestampDoc)TimestampManager.getTimestampsDoc(infoUt, (DocsPaVO.documento.Documento)doc.documenti[0])[0];
                xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/RiferimentoTemporale").InnerText = this.formatDateRifTemp(ts.DTA_CREAZIONE);
                xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/DescrizioneRiferimentoTemporale").InnerText = "Marca temporale";
            }
            */

            if (((DocsPaVO.documento.Documento)doc.documenti[0]).firmato.Equals("1"))
            {
                string descrizioneRif = string.Empty;
                string dataRifTemporale = this.getRiferimentoTemporale(doc, infoUt, out descrizioneRif);
                xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/RiferimentoTemporale").InnerText = dataRifTemporale;
                xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/DescrizioneRiferimentoTemporale").InnerText = descrizioneRif;
            }
            else
            {
                XmlNode n = xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente");
                n.RemoveChild(n.SelectSingleNode("RiferimentoTemporale"));
                n.RemoveChild(n.SelectSingleNode("DescrizioneRiferimentoTemporale"));
            }


            switch (tipo)
            {
                #region Documento Protocollato
                case TipologiaUnitaDocumentaria.DocProtocollato:

                    nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = doc.protocollo.numero;
                    nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = doc.protocollo.anno;
                    nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = string.Format("{0} - Protocollo", this.replaceSpecialCharsHeader(doc.registro.codRegistro));
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Documento Protocollato";

                    nodeUD.SelectSingleNode("Data").InnerText = this.formatDate(doc.protocollo.dataProtocollazione);

                    nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo").InnerText = doc.protocollo.numero;
                    nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione").InnerText = doc.protocollo.anno;
                    nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo").InnerText = "Protocollo";
                    nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo").InnerText = doc.protocollo.segnatura;
                    nodeDatiSpecifici.SelectSingleNode("TipoProtocollo").InnerText = doc.tipoProto;
                    nodeDatiSpecifici.SelectSingleNode("CodiceRegistro").InnerText = doc.registro.codRegistro;
                    nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro").InnerText = doc.registro.descrizione;
                    if (!string.IsNullOrEmpty(doc.registro.chaRF) && doc.registro.chaRF.Equals("1"))
                    {
                        nodeDatiSpecifici.SelectSingleNode("CodiceRF").InnerText = doc.registro.codRegistro;
                        nodeDatiSpecifici.SelectSingleNode("DescrizioneRF").InnerText = doc.registro.descrizione;
                    }

                    if (doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
                    {
                        logger.Debug("TIPO: " + doc.tipoProto);
                        if (((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).mittente != null)
                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).mittente.descrizione;

                        if (((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatari != null && ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatari.Count > 0)
                        {
                            logger.Debug("Destinatari");
                            string destinatari = string.Empty;

                            foreach (object item in ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatari)
                            {
                                DocsPaVO.utente.Corrispondente destinatario = (DocsPaVO.utente.Corrispondente)item;
                                if (!string.IsNullOrEmpty(destinatari))
                                    destinatari = destinatari + ";";

                                destinatari = destinatari + destinatario.descrizione;
                            }

                            if (((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatariConoscenza != null && ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatariConoscenza.Count > 0)
                            {
                                foreach (object item in ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatariConoscenza)
                                {
                                    DocsPaVO.utente.Corrispondente destinatario = (DocsPaVO.utente.Corrispondente)item;
                                    if (!string.IsNullOrEmpty(destinatari))
                                        destinatari = destinatari + ";";

                                    destinatari = destinatari + destinatario.descrizione;
                                }
                            }

                            // MEV Gestione lunghezza campo destinatari
                            if (destinatari.Length >= 4000)
                            {
                                string msg = "L'elenco dei destinatari è troncato. Per l'elenco completo vedi annesso file XML - ";
                                destinatari = this.replaceSpecialChars(msg + destinatari);
                                destinatari = destinatari.Replace("&", "&amp;");
                                destinatari = destinatari.Substring(0, 3800);
                                destinatari = destinatari.Replace("&amp;", "&");
                            }

                            nodeDatiSpecifici.SelectSingleNode("Destinatari").InnerText = destinatari;
                        }
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                    }
                    if (doc.tipoProto.Equals("A"))
                    {
                        nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                        if (!string.IsNullOrEmpty(doc.protocollo.descMezzoSpedizione))
                            nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente").InnerText = doc.protocollo.descMezzoSpedizione;
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                        }
                        nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).descrizioneProtocolloMittente;
                        if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente))
                            nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente").InnerText = this.formatDate(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente);
                        else
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));

                        if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo))
                        {
                            string date = ((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo.Trim();
                            if (date.Length < 10)
                                nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date);
                            else
                                nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date.Substring(0, 10));
                            if (date.Length > 11)
                            {
                                nodeDatiSpecifici.SelectSingleNode("OraArrivo").InnerText = this.formatTime(date.Substring(11, date.Length - 11)); //.Replace(":", ".");
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                            }
                        }
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                        }
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Destinatari"));
                    }
                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre"));
                    nodeDatiSpecifici.SelectSingleNode("DataCreazioneProfiloDocumento").InnerText = this.formatDate(doc.dataCreazione);

                    /*
                    if (numMarche == 0)
                    {
                        xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/RiferimentoTemporale").InnerText = this.formatDateRifTemp(doc.protocollo.dataProtocollazione);
                        xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/DescrizioneRiferimentoTemporale").InnerText = "Data di protocollazione";
                    }
                    */

                    break;
                #endregion

                #region Documento Repertoriato
                case TipologiaUnitaDocumentaria.DocRepertoriato:

                    logger.Debug("Documento Repertoriato");
                    string data = string.Empty;
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggContatore = this.getContatoreRepertorio(doc);

                    string repertorio = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, amm.Codice, false, out data);

                    nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = oggContatore.VALORE_DATABASE;
                    nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = oggContatore.ANNO;

                    //string tipoRegistro = doc.template.DESCRIZIONE + " - " + amm.Codice; // PER TEST CONFIGURAZIONE COPPARI
                    string tipoRegistro = string.Empty;
                    if (oggContatore.TIPO_CONTATORE.Equals("T"))
                    {
                        tipoRegistro = doc.template.DESCRIZIONE;
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                    }
                    if (oggContatore.TIPO_CONTATORE.Equals("A"))
                    {
                        logger.Debug("contatore AOO");
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggContatore.ID_AOO_RF);
                        tipoRegistro = reg.codRegistro + " - " + doc.template.DESCRIZIONE;
                        //tipoRegistro = tipoRegistro + " - " + reg.codRegistro;
                        if (!reg.Equals(null))
                        {
                            nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP").InnerText = reg.codRegistro;
                            nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP").InnerText = reg.descrizione;
                        }
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        }
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                    }
                    if (oggContatore.TIPO_CONTATORE.Equals("R"))
                    {
                        logger.Debug("contatore RF");
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggContatore.ID_AOO_RF);
                        // MODIFICA 01-07-2015 per repertori RF
                        //tipoRegistro = reg.codRegistro + " - " + doc.template.DESCRIZIONE;
                        tipoRegistro = doc.template.DESCRIZIONE;
                        nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = reg.codRegistro + " - " + oggContatore.VALORE_DATABASE;
                        if (!reg.Equals(null))
                        {
                            nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP").InnerText = reg.codRegistro;
                            nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP").InnerText = reg.descrizione;
                        }
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                        }
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                    }

                    nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = this.replaceSpecialCharsHeader(tipoRegistro);
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Documento Repertoriato";

                    nodeUD.SelectSingleNode("Data").InnerText = this.formatDate(oggContatore.DATA_INSERIMENTO);
                    nodeDatiSpecifici.SelectSingleNode("SegnaturaRepertorio").InnerText = repertorio;

                    logger.Debug("selezione tipo proto");

                    // 22-09-2015
                    // Aggiunto ulteriore controllo su predisposti (protocollo != null)
                    if ((!doc.tipoProto.Equals("G")) && doc.protocollo != null && doc.protocollo.segnatura != null)
                    {
                        nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo").InnerText = doc.protocollo.numero;
                        nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione").InnerText = doc.protocollo.anno;
                        nodeDatiSpecifici.SelectSingleNode("DataProtocollazione").InnerText = this.formatDate(doc.protocollo.dataProtocollazione);
                        nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo").InnerText = string.Format("{0} - Protocollo", doc.registro.codRegistro);
                        nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo").InnerText = doc.protocollo.segnatura;
                        nodeDatiSpecifici.SelectSingleNode("TipoProtocollo").InnerText = doc.tipoProto;
                        nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT").InnerText = doc.registro.codRegistro;
                        nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT").InnerText = doc.registro.descrizione;


                        if (doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
                        {

                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).mittente.descrizione;

                            if (((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatari != null && ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatari.Count > 0)
                            {
                                string destinatari = string.Empty;

                                foreach (object item in ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).destinatari)
                                {
                                    DocsPaVO.utente.Corrispondente destinatario = (DocsPaVO.utente.Corrispondente)item;
                                    if (!string.IsNullOrEmpty(destinatari))
                                        destinatari = destinatari + ";";

                                    destinatari = destinatari + destinatario.descrizione;
                                }

                                // MEV Gestione lunghezza campo destinatari
                                if (destinatari.Length >= 4000)
                                {
                                    string msg = "L'elenco dei destinatari è troncato. Per l'elenco completo vedi annesso file XML - ";
                                    destinatari = this.replaceSpecialChars(msg + destinatari);
                                    destinatari = destinatari.Replace("&", "&amp;");
                                    destinatari = destinatari.Substring(0, 3800);
                                    destinatari = destinatari.Replace("&amp;", "&");
                                }

                                nodeDatiSpecifici.SelectSingleNode("Destinatari").InnerText = destinatari;
                            }

                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                        }
                        if (doc.tipoProto.Equals("A"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                            if (!string.IsNullOrEmpty(doc.protocollo.descMezzoSpedizione))
                                nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente").InnerText = doc.protocollo.descMezzoSpedizione;
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                            }
                            nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).descrizioneProtocolloMittente;

                            if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente))
                                nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente").InnerText = this.formatDate(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente);
                            else
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));

                            if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo))
                            {
                                string date = ((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo;
                                nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date);
                                string time = this.getTime(date);
                                if (!string.IsNullOrEmpty(time))
                                {
                                    nodeDatiSpecifici.SelectSingleNode("OraArrivo").InnerText = this.formatTime(time);
                                }
                                else
                                {
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                                }

                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                            }
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Destinatari"));
                        }

                        // se il documento è protocollato devo riportare le informazioni sulla protocollazione nell'elemento DocumentiCollegati
                        XmlElement docCollProto = xmlDoc.CreateElement("DocumentoCollegato");
                        XmlElement chiaveColl = xmlDoc.CreateElement("ChiaveCollegamento");
                        XmlElement numeroColl = xmlDoc.CreateElement("Numero");
                        XmlElement annoColl = xmlDoc.CreateElement("Anno");
                        XmlElement tipoRegColl = xmlDoc.CreateElement("TipoRegistro");
                        XmlElement descrColl = xmlDoc.CreateElement("DescrizioneCollegamento");

                        numeroColl.InnerText = doc.protocollo.numero;
                        annoColl.InnerText = doc.protocollo.anno;
                        tipoRegColl.InnerText = string.Format("{0} - Protocollo", doc.registro.codRegistro);
                        descrColl.InnerText = "Registrazione di protocollo";

                        chiaveColl.AppendChild(numeroColl);
                        chiaveColl.AppendChild(annoColl);
                        chiaveColl.AppendChild(tipoRegColl);
                        docCollProto.AppendChild(chiaveColl);
                        docCollProto.AppendChild(descrColl);

                        xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentiCollegati").AppendChild(docCollProto);

                    }
                    else
                    {
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocollazione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipoProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Mittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Destinatari"));

                    }

                    /*
                    if (numMarche == 0)
                    {
                        xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/RiferimentoTemporale").InnerText = this.formatDateRifTemp(oggContatore.DATA_INSERIMENTO);
                        xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente/DescrizioneRiferimentoTemporale").InnerText = "Data di repertoriazione";
                    }
                    */

                    nodeDatiSpecifici.SelectSingleNode("DataCreazioneProfiloDocumento").InnerText = this.formatDate(doc.dataCreazione);
                    nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre").InnerText = doc.template.DESCRIZIONE;

                    break;
                #endregion

                #region Documento Non Protocollato
                case TipologiaUnitaDocumentaria.DocNP:
                    nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = doc.systemId;
                    nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = this.getDate(doc.dataCreazione.Trim()).Split('/').Last();
                    nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = "PITre";
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Documento Non Protocollato";

                    nodeUD.SelectSingleNode("Data").InnerText = this.formatDate(doc.dataCreazione);

                    if (doc.template != null && doc.template.INVIO_CONSERVAZIONE != null && doc.template.INVIO_CONSERVAZIONE.Equals("1"))
                    {
                        nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre").InnerText = doc.template.DESCRIZIONE;
                    }
                    else
                    {
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre"));
                    }

                    /*
                    if (numMarche == 0)
                    {
                        XmlNode node = xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente");
                        node.RemoveChild(node.SelectSingleNode("RiferimentoTemporale"));
                        node.RemoveChild(node.SelectSingleNode("DescrizioneRiferimentoTemporale"));
                    }
                    */

                    break;
                #endregion

                #region Registro
                case TipologiaUnitaDocumentaria.Registro:
                    logger.Debug("TIPOLOGIA: REGISTRO");
                    nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = doc.systemId;
                    nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = this.getDate(doc.dataCreazione.Trim()).Split('/').Last();
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Stampa registro";

                    // Registri di protocollo
                    if (doc.tipoProto.Equals("R"))
                    {
                        DocsPaVO.areaConservazione.StampaRegistro stampa = this.getInfoStampaRegistro(doc.systemId);

                        DocsPaVO.amministrazione.OrgRegistro reg = BusinessLogic.Amministrazione.RegistroManager.GetRegistro(stampa.idRegistro);
                        logger.Debug("Estrazione ProtoStart e ProtoEnd");
                        SchedaDocumento protoStart = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUt, BusinessLogic.Documenti.DocManager.GetDocNumber(amm.IDAmm, reg.Codice, stampa.numProtoStart, stampa.anno));
                        SchedaDocumento protoEnd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUt, BusinessLogic.Documenti.DocManager.GetDocNumber(amm.IDAmm, reg.Codice, stampa.numProtoEnd, stampa.anno));
                        // ALTRA CABLATURA PER PROBLEMI IN TEST
                        //if (string.IsNullOrEmpty(stampa.numProtoStart))
                        //    stampa.numProtoStart = "1";
                        //if (string.IsNullOrEmpty(stampa.numProtoEnd))
                        //    stampa.numProtoEnd = "100";

                        string tipoReg = "Registro giornale di protocollo - " + BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro).codRegistro;
                        nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = this.replaceSpecialCharsHeader(tipoReg);
                        nodeDatiSpecifici.SelectSingleNode("TipoRegistro").InnerText = this.replaceSpecialCharsHeader(tipoReg);
                        //nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Registro giornale di protocollo - " + BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro).codRegistro;

                        nodeDatiSpecifici.SelectSingleNode("FrequenzaDiStampa").InnerText = "GIORNALIERA";
                        nodeDatiSpecifici.SelectSingleNode("PrimoElementoRegistrato").InnerText = stampa.numProtoStart;
                        nodeDatiSpecifici.SelectSingleNode("UltimoElementoRegistrato").InnerText = stampa.numProtoEnd;
                        nodeDatiSpecifici.SelectSingleNode("DataPrimaRegistrazione").InnerText = this.formatDate(protoStart.protocollo.dataProtocollazione);
                        nodeDatiSpecifici.SelectSingleNode("DataUltimaRegistrazione").InnerText = this.formatDate(protoEnd.protocollo.dataProtocollazione);

                        logger.Debug("Ruolo Responsabile");
                        if (reg != null && !string.IsNullOrEmpty(reg.idRuoloResp))
                        {
                            nodeDatiSpecifici.SelectSingleNode("RuoloResponsabileRegistro").InnerText = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(reg.idRuoloResp).descrizione;
                        }
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("RuoloResponsabileRegistro"));
                        }

                        // MODIFICA 01-07-2015 per repertori RF
                        logger.Debug("Mod rep RF");
                        nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT").InnerText = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro).codRegistro;
                        nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT").InnerText = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro).descrizione;
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                    }
                    // Registri di repertorio
                    if (doc.tipoProto.Equals("C"))
                    {

                        DocsPaVO.areaConservazione.StampaRegistro stampa = this.getInfoStampaRepertorio(doc.systemId);

                        // recupero il contatore
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getOggettoById(stampa.idRepertorio);

                        // recupero la descrizione della tipologia
                        string tipologia = this.replaceSpecialCharsHeader(this.getNomeTipologia(contatore.SYSTEM_ID.ToString()));

                        // se il repertorio è di AOO/RF, recupero il registro/RF associato
                        DocsPaVO.utente.Registro reg = null;
                        DocsPaVO.utente.Repertori.RegistroRepertorioSingleSettings settings = new DocsPaVO.utente.Repertori.RegistroRepertorioSingleSettings();

                        if (!(contatore.TIPO_CONTATORE.Equals("T")))
                        {
                            reg = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro);

                            if (reg.chaRF.Equals("1"))
                            {
                                // RF
                                logger.Debug("RF");
                                settings = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetRegisterSettings(stampa.idRepertorio, "", reg.systemId, DocsPaVO.utente.Repertori.RegistroRepertorio.TipologyKind.D, DocsPaVO.utente.Repertori.RegistroRepertorio.SettingsType.S);
                            }
                            else
                            {
                                // AOO
                                logger.Debug("AOO");

                                settings = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetRegisterSettings(stampa.idRepertorio, reg.systemId, "", DocsPaVO.utente.Repertori.RegistroRepertorio.TipologyKind.D, DocsPaVO.utente.Repertori.RegistroRepertorio.SettingsType.S);

                                if (!(settings != null))
                                {
                                    settings = new DocsPaVO.utente.Repertori.RegistroRepertorioSingleSettings();
                                    settings.RoleAndUserDescription = string.Empty;
                                    settings.LongPrintFrequency = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            // TIPOLOGIA
                            settings = BusinessLogic.utenti.RegistriRepertorioPrintManager.GetRegisterSettings(stampa.idRepertorio, "", "", DocsPaVO.utente.Repertori.RegistroRepertorio.TipologyKind.D, DocsPaVO.utente.Repertori.RegistroRepertorio.SettingsType.S);
                        }

                        if (settings != null && contatore != null)
                        {
                            logger.Debug("Impostazione dati specifici contatore");
                            // imposto il tipo registro
                            if (contatore.TIPO_CONTATORE.Equals("T"))
                            {
                                //nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = string.Format("Repertorio - {0}", contatore.DESCRIZIONE);
                                nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = string.Format("Repertorio - {0}", tipologia);
                                nodeDatiSpecifici.SelectSingleNode("TipoRegistro").InnerText = string.Format("Repertorio - {0}", tipologia);
                                // MODIFICA 01-07-2015 per repertori RF
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                            }
                            else
                            {
                                if (contatore.TIPO_CONTATORE.Equals("A"))
                                {
                                    //nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = string.Format("Repertorio - {0} - {1}", reg.codRegistro, contatore.DESCRIZIONE);
                                    nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = string.Format("Repertorio - {0} - {1}", this.replaceSpecialCharsHeader(reg.codRegistro), tipologia);
                                    nodeDatiSpecifici.SelectSingleNode("TipoRegistro").InnerText = string.Format("Repertorio - {0} - {1}", this.replaceSpecialCharsHeader(reg.codRegistro), tipologia);

                                    // MODIFICA 01-07-2015 per repertori RF
                                    nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP").InnerText = reg.codRegistro;
                                    nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP").InnerText = reg.descrizione;
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT"));
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT"));
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                                }
                                else
                                {
                                    // MODIFICA 01-07-2015 per repertori RF
                                    //nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = string.Format("Repertorio - {0}", contatore.DESCRIZIONE);
                                    nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = string.Format("Repertorio - {0}", tipologia);
                                    nodeDatiSpecifici.SelectSingleNode("TipoRegistro").InnerText = string.Format("Repertorio - {0}", tipologia);

                                    nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = reg.codRegistro + " - " + doc.systemId;

                                    // MODIFICA 01-07-2015 per repertori RF
                                    nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP").InnerText = reg.codRegistro;
                                    nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP").InnerText = reg.descrizione;
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT"));
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT"));
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                                }
                            }

                            // CABLATI------------------
                            //nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Registro di repertorio";
                            //nodeDatiSpecifici.SelectSingleNode("TipoRegistro").InnerText = "Registro di repertorio";
                            // -------------------------

                            if (!string.IsNullOrEmpty(settings.RoleAndUserDescription))
                                nodeDatiSpecifici.SelectSingleNode("RuoloResponsabileRegistro").InnerText = settings.RoleAndUserDescription.Split('-')[0].Trim();
                            else
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("RuoloResponsabileRegistro"));

                            if (!string.IsNullOrEmpty(settings.LongPrintFrequency))
                                nodeDatiSpecifici.SelectSingleNode("FrequenzaDiStampa").InnerText = settings.LongPrintFrequency;
                            else
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("FrequenzaDiStampa"));
                        }
                        else
                        {

                        }

                        nodeDatiSpecifici.SelectSingleNode("PrimoElementoRegistrato").InnerText = stampa.numProtoStart;
                        nodeDatiSpecifici.SelectSingleNode("UltimoElementoRegistrato").InnerText = stampa.numProtoEnd;
                        string firstDate = this.getDataRepertoriazione(stampa.anno, stampa.numProtoStart, stampa.idRepertorio);
                        string lastDate = this.getDataRepertoriazione(stampa.anno, stampa.numProtoEnd, stampa.idRepertorio);
                        if (string.IsNullOrEmpty(firstDate))
                            firstDate = string.Format("01/01/{0}", stampa.anno);
                        if (string.IsNullOrEmpty(lastDate))
                            lastDate = string.Format("31/12/{0}", stampa.anno);
                        //nodeDatiSpecifici.SelectSingleNode("DataPrimaRegistrazione").InnerText = this.formatDate(this.getDataRepertoriazione(stampa.anno, stampa.numProtoStart, stampa.idRepertorio));
                        //nodeDatiSpecifici.SelectSingleNode("DataUltimaRegistrazione").InnerText = this.formatDate(this.getDataRepertoriazione(stampa.anno, stampa.numProtoEnd, stampa.idRepertorio));
                        nodeDatiSpecifici.SelectSingleNode("DataPrimaRegistrazione").InnerText = this.formatDate(firstDate);
                        nodeDatiSpecifici.SelectSingleNode("DataUltimaRegistrazione").InnerText = this.formatDate(lastDate);

                    }

                    nodeUD.SelectSingleNode("Data").InnerText = this.formatDate(doc.dataCreazione);

                    /*
                    if (numMarche.Equals(0))
                    {
                        XmlNode node = xmlDoc.SelectSingleNode("UnitaDocumentaria/DocumentoPrincipale/StrutturaOriginale/Componenti/Componente");
                        node.RemoveChild(node.SelectSingleNode("RiferimentoTemporale"));
                        node.RemoveChild(node.SelectSingleNode("DescrizioneRiferimentoTemporale"));
                    }
                    */

                    break;
                #endregion

                #region Fattura elettronica
                case TipologiaUnitaDocumentaria.FatturaElettronica:
                    logger.Debug("Fattura Elettronica");
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "FATTURA PASSIVA";
                    #region analisi protocollo
                    if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                    {
                        logger.Debug("Fattura protocollata");
                        nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo").InnerText = doc.protocollo.numero;
                        nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione").InnerText = doc.protocollo.anno;
                        nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo").InnerText = "Protocollo";
                        nodeDatiSpecifici.SelectSingleNode("DataProtocollazione").InnerText = this.formatDate(doc.protocollo.dataProtocollazione);
                        
                        nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo").InnerText = doc.protocollo.segnatura;
                        nodeDatiSpecifici.SelectSingleNode("TipoProtocollo").InnerText = doc.tipoProto;
                        nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT").InnerText = doc.registro.codRegistro;
                        nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT").InnerText = doc.registro.descrizione;
                        if (!string.IsNullOrEmpty(doc.registro.chaRF) && doc.registro.chaRF.Equals("1"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT").InnerText = doc.registro.codRegistro;
                            nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT").InnerText = doc.registro.descrizione;
                        }
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT"));
                        }
                        if (doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).mittente.descrizione;

                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                        }
                        if (doc.tipoProto.Equals("A"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                            if (!string.IsNullOrEmpty(doc.protocollo.descMezzoSpedizione))
                                nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente").InnerText = doc.protocollo.descMezzoSpedizione;
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                            }
                            nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).descrizioneProtocolloMittente;
                            if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente))
                                nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente").InnerText = this.formatDate(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente);
                            else
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));

                            if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo))
                            {
                                string date = ((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo.Trim();
                                if (date.Length < 10)
                                    nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date);
                                else
                                    nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date.Substring(0, 10));
                                if (date.Length > 11)
                                {
                                    nodeDatiSpecifici.SelectSingleNode("OraArrivo").InnerText = this.formatTime(date.Substring(11, date.Length - 11)); //.Replace(":", ".");
                                }
                                else
                                {
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                                }
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                            }

                        }

                    }
                    else
                    {
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocollazione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipoProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Mittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));

                    }
                    #endregion
                    #region analisi repertorio
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatoreX = this.getContatoreRepertorio(doc);
                    if (contatoreX != null && contatoreX.CONS_REPERTORIO != null && contatoreX.CONS_REPERTORIO.Equals("1"))
                    {
                        logger.Debug("Fattura repertoriata");
                        nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "FATTURA PASSIVA";
                        nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "FATTURA PASSIVA";
                        string repertorioX = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, amm.Codice, false, out data);

                        nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = contatoreX.VALORE_DATABASE;
                        nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = contatoreX.ANNO;

                        nodeDatiSpecifici.SelectSingleNode("NumeroRepertorio").InnerText = contatoreX.VALORE_DATABASE;
                        nodeDatiSpecifici.SelectSingleNode("DataRepertorio").InnerText = this.formatDate(contatoreX.DATA_INSERIMENTO);

                        // Questo serve per versamenti di fatture vecchie nel caso in cui il tipo sia stato cambiato da AOO a RF (o viceversa)
                        try
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX.ID_AOO_RF);
                            logger.DebugFormat("Tipo contatore: {0} - ID registro {1} - chaRF={2} ", contatoreX.TIPO_CONTATORE, reg.systemId, reg.chaRF);
                            if (contatoreX.TIPO_CONTATORE.Equals("A") && reg.chaRF.Equals("1"))
                            {
                                contatoreX.TIPO_CONTATORE = "R";
                                logger.Debug("TipoContatore modificato in R");
                            }
                            else if (contatoreX.TIPO_CONTATORE.Equals("R") && reg.chaRF.Equals("0"))
                            {
                                contatoreX.TIPO_CONTATORE = "A";
                                logger.Debug("TipoContatore modificato in A");
                            }
                            
                        }
                        catch (Exception exReg)
                        {
                            logger.Debug(exReg.Message);
                        }

                        string tipoRegistroX = string.Empty;
                        if (contatoreX.TIPO_CONTATORE.Equals("T"))
                        {
                            tipoRegistroX = doc.template.DESCRIZIONE;
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        }
                        if (contatoreX.TIPO_CONTATORE.Equals("A"))
                        {
                            logger.Debug("contatore AOO");
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX.ID_AOO_RF);
                            tipoRegistroX = reg.codRegistro + " - " + doc.template.DESCRIZIONE;
                            //tipoRegistro = tipoRegistro + " - " + reg.codRegistro;
                            if (!reg.Equals(null))
                            {
                                nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP").InnerText = reg.codRegistro;
                                nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP").InnerText = reg.descrizione;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                            }
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                        }
                        if (contatoreX.TIPO_CONTATORE.Equals("R"))
                        {
                            logger.Debug("contatore RF");
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX.ID_AOO_RF);
                            // MODIFICA 01-07-2015 per repertori RF
                            //tipoRegistroX = reg.codRegistro + " - " + doc.template.DESCRIZIONE;
                            tipoRegistroX = doc.template.DESCRIZIONE;
                            nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = reg.codRegistro + " - " + contatoreX.VALORE_DATABASE;
                            //nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = contatoreX.VALORE_DATABASE;
                            
                            if (!reg.Equals(null))
                            {
                                nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP").InnerText = reg.codRegistro;
                                nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP").InnerText = reg.descrizione;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                            }
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        }

                        nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = this.replaceSpecialCharsHeader(tipoRegistroX);
                        //nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "Documento Repertoriato";

                        nodeUD.SelectSingleNode("Data").InnerText = this.formatDate(contatoreX.DATA_INSERIMENTO);
                        nodeDatiSpecifici.SelectSingleNode("SegnaturaRepertorio").InnerText = repertorioX;
                    }
                    else
                    {
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("SegnaturaRepertorio"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroRepertorio"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataRepertorio"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                        nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = doc.systemId;
                        nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = this.getDate(doc.dataCreazione.Trim()).Split('/').Last();
                        nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = "PITre";
                    }
                    #endregion
                    #region analisi tipologia
                    if (doc.template != null && doc.template.DESCRIZIONE != null)
                    {
                        nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre").InnerText = doc.template.DESCRIZIONE;
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in doc.template.ELENCO_OGGETTI)
                        {
                            if (!string.IsNullOrEmpty(oggetto.DESCRIZIONE))
                            {
                                string descOggetto = oggetto.DESCRIZIONE.ToUpper();
                                switch (descOggetto)
                                {
                                    case "DATA EMISSIONE":
                                        nodeDatiSpecifici.SelectSingleNode("DataEmissione").InnerText = this.getValoreOggettoCustom(oggetto);
                                        break;
                                    case "NUMERO FATTURA":
                                        nodeDatiSpecifici.SelectSingleNode("NumeroEmissione").InnerText = this.getValoreOggettoCustom(oggetto);
                                        break;
                                    case "FORNITORE":
                                        nodeDatiSpecifici.SelectSingleNode("DenominazioneMittente").InnerText = this.getValoreOggettoCustom(oggetto);
                                        break;
                                    case "COD. FORNITORE":
                                    case "PARTITA IVA FORNITORE":
                                        nodeDatiSpecifici.SelectSingleNode("PartitaIvaMittente").InnerText = this.getValoreOggettoCustom(oggetto);
                                        break;
                                    case "CODICE FISCALE FORNITORE":
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("CodiceFiscaleMittente").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceFiscaleMittente"));
                                        break;
                                    case "CODICE CUP":
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("CUP").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CUP"));
                                        break;
                                    case "CODICE CIG":
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("CIG").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CIG"));
                                        break;
                                    case "IDENTIFICATIVO SDI":
                                        //nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI").InnerText = this.getValoreOggettoCustom(oggetto);
                                        //break;
                                         if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                             nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                             nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI"));
                                        break;
                                    case "ALIQUOTAIVAREVERSECHARGE":
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("AliquotaIvaReverseCharge").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("AliquotaIvaReverseCharge"));
                                        break;
                                    case "IVATOTALEREVERSECHARGE":
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("IvaTotaleReverseCharge").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("IvaTotaleReverseCharge"));
                                        break;
                                }
                            }
                        }
                        XmlNodeList nodesXFatt = nodeDatiSpecifici.SelectNodes(@"//*[not(node())]");
                        if (nodesXFatt.Count > 0)
                        {
                            logger.Debug("presenti nodi vuoti");
                            for (int i = nodesXFatt.Count - 1; i >= 0; i--)
                            {
                                nodesXFatt[i].ParentNode.RemoveChild(nodesXFatt[i]);
                            }
                        }
                    }
                    else
                    {
                        // questo pezzo dovrebbe essere inutile
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroEmissione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataEmissione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DenominazioneMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("PartitaIvaMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceFiscaleMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CIG"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CUP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("AliquotaIvaReverseCharge"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("IvaTotaleReverseCharge"));

                    }
                    #endregion

                    break;
                #endregion

                #region Lotto di fatture
                case TipologiaUnitaDocumentaria.LottoDiFatture:
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "LOTTO DI FATTURE";
                    #region analisi protocollo
                    if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.numero))
                    {
                        logger.Debug("Fattura protocollata");
                        nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo").InnerText = doc.protocollo.numero;
                        nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione").InnerText = doc.protocollo.anno;
                        nodeDatiSpecifici.SelectSingleNode("DataProtocollazione").InnerText = this.formatDate(doc.protocollo.dataProtocollazione);
                        
                        nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo").InnerText = "Protocollo";
                        nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo").InnerText = doc.protocollo.segnatura;
                        nodeDatiSpecifici.SelectSingleNode("TipoProtocollo").InnerText = doc.tipoProto;
                        nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT").InnerText = doc.registro.codRegistro;
                        nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT").InnerText = doc.registro.descrizione;
                        if (!string.IsNullOrEmpty(doc.registro.chaRF) && doc.registro.chaRF.Equals("1"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT").InnerText = doc.registro.codRegistro;
                            nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT").InnerText = doc.registro.descrizione;
                        }
                        else
                        {
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT"));
                        }
                        if (doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloUscita)doc.protocollo).mittente.descrizione;

                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                        }
                        if (doc.tipoProto.Equals("A"))
                        {
                            nodeDatiSpecifici.SelectSingleNode("Mittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente.descrizione;
                            if (!string.IsNullOrEmpty(doc.protocollo.descMezzoSpedizione))
                                nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente").InnerText = doc.protocollo.descMezzoSpedizione;
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                            }
                            nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente").InnerText = ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).descrizioneProtocolloMittente;
                            if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente))
                                nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente").InnerText = this.formatDate(((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).dataProtocolloMittente);
                            else
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));

                            if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo))
                            {
                                string date = ((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo.Trim();
                                if (date.Length < 10)
                                    nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date);
                                else
                                    nodeDatiSpecifici.SelectSingleNode("DataArrivo").InnerText = this.formatDate(date.Substring(0, 10));
                                if (date.Length > 11)
                                {
                                    nodeDatiSpecifici.SelectSingleNode("OraArrivo").InnerText = this.formatTime(date.Substring(11, date.Length - 11)); //.Replace(":", ".");
                                }
                                else
                                {
                                    nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                                }
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));
                            }

                        }

                    }
                    else
                    {
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("AnnoProtocollazione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocollazione"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipoRegistroProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("SegnaturaProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipoProtocollo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_PROT"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Mittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("MezzoDiSpedizioneMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("ProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataProtocolloMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataArrivo"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("OraArrivo"));

                    }
                    #endregion
                    #region analisi repertorio
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatoreX2 = this.getContatoreRepertorio(doc);
                    if (contatoreX2 != null && contatoreX2.CONS_REPERTORIO != null && contatoreX2.CONS_REPERTORIO.Equals("1"))
                    {
                        logger.Debug("Fattura repertoriata");
                        nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "LOTTO DI FATTURE";
                        nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "LOTTO DI FATTURE";
                        string repertorioX = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, amm.Codice, false, out data);

                        nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = contatoreX2.VALORE_DATABASE;
                        nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = contatoreX2.ANNO;

                        nodeDatiSpecifici.SelectSingleNode("NumeroRepertorio").InnerText = contatoreX2.VALORE_DATABASE;
                        nodeDatiSpecifici.SelectSingleNode("DataRepertorio").InnerText = this.formatDate(contatoreX2.DATA_INSERIMENTO);

                        string tipoRegistroX = string.Empty;
                        if (contatoreX2.TIPO_CONTATORE.Equals("T"))
                        {
                            tipoRegistroX = doc.template.DESCRIZIONE;
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        }
                        if (contatoreX2.TIPO_CONTATORE.Equals("A"))
                        {
                            logger.Debug("contatore AOO");
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX2.ID_AOO_RF);
                            tipoRegistroX = reg.codRegistro + " - " + doc.template.DESCRIZIONE;
                            //tipoRegistro = tipoRegistro + " - " + reg.codRegistro;
                            if (!reg.Equals(null))
                            {
                                nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP").InnerText = reg.codRegistro;
                                nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP").InnerText = reg.descrizione;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                            }
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                        }
                        if (contatoreX2.TIPO_CONTATORE.Equals("R"))
                        {
                            logger.Debug("contatore RF");
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX2.ID_AOO_RF);
                            // MODIFICA 01-07-2015 per repertori RF
                            //tipoRegistroX = reg.codRegistro + " - " + doc.template.DESCRIZIONE;
                            tipoRegistroX = doc.template.DESCRIZIONE;
                            nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = reg.codRegistro + " - " + contatoreX2.VALORE_DATABASE;
                            //nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = contatoreX2.VALORE_DATABASE;
                            if (!reg.Equals(null))
                            {
                                nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP").InnerText = reg.codRegistro;
                                nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP").InnerText = reg.descrizione;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                            }
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        }

                        nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = this.replaceSpecialCharsHeader(tipoRegistroX);
                        nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "LOTTO DI FATTURE";

                        nodeUD.SelectSingleNode("Data").InnerText = this.formatDate(contatoreX2.DATA_INSERIMENTO);
                        nodeDatiSpecifici.SelectSingleNode("SegnaturaRepertorio").InnerText = repertorioX;
                    }
                    else
                    {
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("SegnaturaRepertorio"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroRepertorio"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DataRepertorio"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRegistro_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceRF_REP"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneRF_REP"));
                        nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = doc.systemId;
                        nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = this.getDate(doc.dataCreazione.Trim()).Split('/').Last();
                        nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = "PITre";
                    }
                    #endregion
                    #region analisi tipologia
                    if (doc.template != null && doc.template.DESCRIZIONE != null)
                    {
                        nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre").InnerText = doc.template.DESCRIZIONE;
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in doc.template.ELENCO_OGGETTI)
                        {
                            if (!string.IsNullOrEmpty(oggetto.DESCRIZIONE))
                            {
                                string descOggetto = oggetto.DESCRIZIONE.ToUpper();
                                switch (descOggetto)
                                {
                                    case "FORNITORE":
                                        nodeDatiSpecifici.SelectSingleNode("DenominazioneMittente").InnerText = this.getValoreOggettoCustom(oggetto);
                                        break;
                                    case "COD. FORNITORE":
                                    case "PARTITA IVA FORNITORE":
                                        nodeDatiSpecifici.SelectSingleNode("PartitaIvaMittente").InnerText = this.getValoreOggettoCustom(oggetto);
                                        break;
                                    case "CODICE FISCALE FORNITORE":
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("CodiceFiscaleMittente").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceFiscaleMittente"));
                                        break;
                                    case "IDENTIFICATIVO SDI":
                                        //nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI").InnerText = this.getValoreOggettoCustom(oggetto);
                                        //break;
                                        if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                            nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI").InnerText = this.getValoreOggettoCustom(oggetto);
                                        else
                                            nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI"));
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // questo pezzo dovrebbe essere inutile
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("TipologiaDocumentalePITre"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DenominazioneMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("PartitaIvaMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("CodiceFiscaleMittente"));
                        nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("IdentificativoSdI"));
                    }
                    #endregion

                    break;
                #endregion

                #region Verbale sintentico di seduta
                case TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta:
                    string codiceEnte = string.Empty;
                    string codiceOrgano = string.Empty;
                    string numeroSeduta = string.Empty;
                    string dataSeduta = string.Empty;
                    string annoSeduta = string.Empty;

                    #region Analisi campi profilati
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in doc.template.ELENCO_OGGETTI)
                    {
                        if (ogg.DESCRIZIONE.ToUpper() == "ENTE ESPRESSO CON CODICE IPA")
                        {
                            if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                            {
                                codiceEnte = ogg.VALORE_DATABASE;
                                nodeDatiSpecifici.SelectSingleNode("Ente").InnerText = ogg.VALORE_DATABASE;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Ente"));
                            }
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "ORGANO")
                        {
                            if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                            {
                                codiceOrgano = ogg.VALORE_DATABASE;
                                nodeDatiSpecifici.SelectSingleNode("Organo").InnerText = ogg.VALORE_DATABASE;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("Organo"));
                            }
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "NUMERO SEDUTA")
                        {
                            if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                            {
                                numeroSeduta = ogg.VALORE_DATABASE;
                                nodeDatiSpecifici.SelectSingleNode("NumeroSeduta").InnerText = ogg.VALORE_DATABASE;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("NumeroSeduta"));
                            }
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "DATA SEDUTA")
                        {
                            if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                            {
                                
                                nodeDatiSpecifici.SelectSingleNode("DataSeduta").InnerText = ogg.VALORE_DATABASE;

                                // il campo data seduta DOVREBBE essere nel formato YYYY-MM-DD
                                dataSeduta = ogg.VALORE_DATABASE;
                                annoSeduta = dataSeduta.Split('-')[0];

                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DatiSpecifici"));
                            }
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "DESCRIZIONE ENTE")
                        {
                            if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                            {
                                nodeDatiSpecifici.SelectSingleNode("DescrizioneEnte").InnerText = ogg.VALORE_DATABASE;
                            }
                            else
                            {
                                nodeDatiSpecifici.RemoveChild(nodeDatiSpecifici.SelectSingleNode("DescrizioneEnte"));
                            }
                        }
                    }
                    #endregion

                    nodeIntestazione.SelectSingleNode("Chiave/Numero").InnerText = string.Format("{0}-{1}-{2}", codiceEnte, codiceOrgano, numeroSeduta);
                    nodeIntestazione.SelectSingleNode("Chiave/Anno").InnerText = annoSeduta;
                    nodeIntestazione.SelectSingleNode("Chiave/TipoRegistro").InnerText = "VERBALI SINTETICI DI SEDUTA";
                    nodeIntestazione.SelectSingleNode("TipologiaUnitaDocumentaria").InnerText = "VERBALE SINTETICO DI SEDUTA";

                    nodeUD.SelectSingleNode("Data").InnerText = dataSeduta;

                    break;
                #endregion
            }

            logger.Debug("END");
        }

        private string getTemplateXML(string idAmm, string tipo)
        {
            string retVal = string.Empty;

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            retVal = cons.getTemplateXML(idAmm, tipo);

            return retVal;
        }

        private string getDate(string date)
        {

            logger.Debug("get date " + date);
            string retVal = string.Empty;

            if (date.Length < 10)
            {
                retVal = date;
            }
            else
            {
                retVal = date.Substring(0, 10);
            }

            return retVal;
        }

        private string getTime(string date)
        {
            string retVal = string.Empty;

            if (date.Length > 11)
            {
                retVal = date.Substring(11, date.Length - 11).Replace(":", ".");
            }
            else
            {
                retVal = string.Empty;
            }

            return retVal;
        }

        private string getImpronta(DocsPaVO.documento.FileRequest doc)
        {
            string result = string.Empty;
            try
            {
                string data = string.Empty;
                DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                docs.GetImpronta(out result, doc.versionId, doc.docNumber);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        public TipologiaUnitaDocumentaria getTipoDocumento(SchedaDocumento doc, string codiceAmm)
        {

            TipologiaUnitaDocumentaria result = new TipologiaUnitaDocumentaria();

            if (doc.tipoProto.Equals("A") || doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
            {
                string data = string.Empty;

                if (string.IsNullOrEmpty(BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, codiceAmm, false, out data)))
                {
                    // controllo sulla predisposizione
                    if (doc.protocollo != null && !(string.IsNullOrEmpty(doc.protocollo.segnatura)))
                    {
                        result = TipologiaUnitaDocumentaria.DocProtocollato;
                    }
                    else
                    {
                        result = TipologiaUnitaDocumentaria.DocNP;
                    }
                }
                else
                {
                    // verifico se il repertorio è configurato per l'invio in conservazione
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = this.getContatoreRepertorio(doc);

                    if (contatore != null && contatore.CONS_REPERTORIO != null && contatore.CONS_REPERTORIO.Equals("1"))
                        result = TipologiaUnitaDocumentaria.DocRepertoriato;
                    else
                        result = TipologiaUnitaDocumentaria.DocProtocollato;
                }

            }
            else if (doc.tipoProto.Equals("G"))
            {
                string data = string.Empty;

                if (string.IsNullOrEmpty(BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, codiceAmm, false, out data)))
                    result = TipologiaUnitaDocumentaria.DocNP;
                else
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = this.getContatoreRepertorio(doc);
                    if (contatore != null && contatore.CONS_REPERTORIO != null && contatore.CONS_REPERTORIO.Equals("1"))
                        result = TipologiaUnitaDocumentaria.DocRepertoriato;
                    else
                        result = TipologiaUnitaDocumentaria.DocNP;
                }

            }
            else if (doc.tipoProto.Equals("R") || doc.tipoProto.Equals("C"))
            {
                result = TipologiaUnitaDocumentaria.Registro;
            }

            if (doc.template != null && !string.IsNullOrEmpty(doc.template.DESCRIZIONE) && doc.template.INVIO_CONSERVAZIONE != null && doc.template.INVIO_CONSERVAZIONE.Equals("1"))
            {
                if (doc.template.DESCRIZIONE.ToUpper() == "FATTURA ELETTRONICA")
                {
                    result = TipologiaUnitaDocumentaria.FatturaElettronica;
                }
                else if (doc.template.DESCRIZIONE.ToUpper() == "LOTTO DI FATTURE")
                {
                    result = TipologiaUnitaDocumentaria.LottoDiFatture;
                }
                else if (doc.template.DESCRIZIONE.ToUpper() == "VERBALE DI SEDUTA ORGANI") 
                {
                    result = TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta;
                }
            }

            return result;
        }

        public ChiaveVersamento getChiaveVersamento(SchedaDocumento doc, TipologiaUnitaDocumentaria type)
        {
            ChiaveVersamento result = new ChiaveVersamento();

            switch (type)
            {
                case TipologiaUnitaDocumentaria.DocProtocollato:
                    result.numero = doc.protocollo.numero;
                    result.anno = doc.protocollo.anno;
                    result.tipoRegistro = string.Format("{0} - Protocollo", this.replaceSpecialCharsHeader(doc.registro.codRegistro));
                    break;
                case TipologiaUnitaDocumentaria.DocRepertoriato:
                    DocsPaVO.utente.Utente u = BusinessLogic.Utenti.UserManager.getUtenteById(doc.creatoreDocumento.idPeople);
                    DocsPaVO.utente.Ruolo r = BusinessLogic.Utenti.UserManager.getRuolo(doc.creatoreDocumento.idCorrGlob_Ruolo);
                    DocsPaVO.utente.InfoUtente infoUt = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = this.getContatoreRepertorio(doc);
                    string data = string.Empty;
                    result.numero = contatore.VALORE_DATABASE;
                    result.anno = contatore.ANNO;
                    
                    if (contatore.TIPO_CONTATORE.Equals("T"))
                    {
                        result.tipoRegistro = this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                    }
                    else
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatore.ID_AOO_RF);
                        if (contatore.TIPO_CONTATORE.Equals("A"))
                            result.tipoRegistro = this.replaceSpecialCharsHeader(reg.codRegistro) + " - " + this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                        else
                        {
                            // MODIFICA 01-07-2015 per repertori RF
                            result.tipoRegistro = this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                            result.numero = reg.codRegistro + " - " + contatore.VALORE_DATABASE;
                        }
                    }
                    break;
                case TipologiaUnitaDocumentaria.DocNP:
                    result.numero = doc.docNumber;
                    result.anno = this.getDate(doc.dataCreazione).Split('/').Last();
                    result.tipoRegistro = "PITre";
                    
                    break;
                case TipologiaUnitaDocumentaria.Registro:
                    result.numero = doc.docNumber;
                    result.anno = this.getDate(doc.dataCreazione).Split('/').Last();
                    if (doc.tipoProto.Equals("R"))
                    {
                        DocsPaVO.areaConservazione.StampaRegistro stampa = this.getInfoStampaRegistro(doc.systemId);
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro);
                        result.tipoRegistro = "Registro giornale di protocollo - " + this.replaceSpecialCharsHeader(reg.codRegistro);
                    }
                    else
                    {
                        // recupero contatore
                        DocsPaVO.areaConservazione.StampaRegistro stampa = this.getInfoStampaRepertorio(doc.systemId);
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getOggettoById(stampa.idRepertorio);

                        // recupero la descrizione della tipologia
                        string tipologia = this.replaceSpecialCharsHeader(this.getNomeTipologia(ogg.SYSTEM_ID.ToString()));

                        if (ogg.TIPO_CONTATORE.Equals("T"))
                        {
                            // tipologia
                            result.tipoRegistro = string.Format("Repertorio - {0}", tipologia);
                        }
                        else
                        {
                            // AOO o RF
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro);
                            if (ogg.TIPO_CONTATORE.Equals("A"))
                                result.tipoRegistro = string.Format("Repertorio - {0} - {1}", this.replaceSpecialCharsHeader(reg.codRegistro), tipologia);
                            else
                            {
                                result.tipoRegistro = string.Format("Repertorio - {0}", tipologia);
                                result.numero = reg.codRegistro + " - " + doc.docNumber;
                            }
                        }

                    }
                    
                    break;

                case TipologiaUnitaDocumentaria.LottoDiFatture :
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatoreX2 = this.getContatoreRepertorio(doc);
                    if (contatoreX2 != null && contatoreX2.CONS_REPERTORIO != null && contatoreX2.CONS_REPERTORIO.Equals("1"))
                    {
                        //DocsPaVO.utente.Utente uLF = BusinessLogic.Utenti.UserManager.getUtenteById(doc.creatoreDocumento.idPeople);
                        //DocsPaVO.utente.Ruolo rLF = BusinessLogic.Utenti.UserManager.getRuolo(doc.creatoreDocumento.idCorrGlob_Ruolo);
                        //DocsPaVO.utente.InfoUtente infoUtLF = BusinessLogic.Utenti.UserManager.GetInfoUtente(uLF, rLF);
                   
                        //string repertorioX = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, amm.Codice, false, out data);
                        result.numero = contatoreX2.VALORE_DATABASE;
                        result.anno = contatoreX2.ANNO;
                        if (contatoreX2.TIPO_CONTATORE.Equals("T"))
                        {
                            result.tipoRegistro = this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                        }
                        else
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX2.ID_AOO_RF);
                            if (contatoreX2.TIPO_CONTATORE.Equals("A"))
                                result.tipoRegistro = this.replaceSpecialCharsHeader(reg.codRegistro) + " - " + this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                            else
                            {
                                // MODIFICA 01-07-2015 per repertori RF
                                result.tipoRegistro = this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                                result.numero = reg.codRegistro + " - " + contatoreX2.VALORE_DATABASE;
                            }
                        }

                    }

                    break;

                case TipologiaUnitaDocumentaria.FatturaElettronica:
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom contatoreX3 = this.getContatoreRepertorio(doc);
                    if (contatoreX3 != null && contatoreX3.CONS_REPERTORIO != null && contatoreX3.CONS_REPERTORIO.Equals("1"))
                    {

                        //string repertorioX = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(doc.docNumber, amm.Codice, false, out data);
                        result.numero = contatoreX3.VALORE_DATABASE;
                        result.anno = contatoreX3.ANNO;
                        if (contatoreX3.TIPO_CONTATORE.Equals("T"))
                        {
                            result.tipoRegistro = this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                        }
                        else
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(contatoreX3.ID_AOO_RF);
                            if (contatoreX3.TIPO_CONTATORE.Equals("A"))
                                result.tipoRegistro = this.replaceSpecialCharsHeader(reg.codRegistro) + " - " + this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                            else
                            {
                                // MODIFICA 01-07-2015 per repertori RF
                                result.tipoRegistro = this.replaceSpecialCharsHeader(doc.template.DESCRIZIONE);
                                result.numero = reg.codRegistro + " - " + contatoreX3.VALORE_DATABASE;
                            }
                        }

                    }

                    break;
                case TipologiaUnitaDocumentaria.VerbaleSinteticoDiSeduta:
                    string codiceEnte = string.Empty;
                    string codiceOrgano = string.Empty;
                    string numeroSeduta = string.Empty;
                    string dataSedutaString = string.Empty;
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in doc.template.ELENCO_OGGETTI)
                    {
                        if (ogg.DESCRIZIONE.ToUpper() == "ENTE ESPRESSO CON CODICE IPA")
                        {
                            codiceEnte = ogg.VALORE_DATABASE;
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "ORGANO")
                        {
                            codiceOrgano = ogg.VALORE_DATABASE;
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "NUMERO SEDUTA")
                        {
                            numeroSeduta = ogg.VALORE_DATABASE;
                        }
                        else if (ogg.DESCRIZIONE.ToUpper() == "DATA SEDUTA")
                        {
                            dataSedutaString = ogg.VALORE_DATABASE;
                        }
                    }

                    DateTime dataSeduta;
                    DateTime.TryParse(dataSedutaString, out dataSeduta);

                    result.numero = codiceEnte + "-" + codiceOrgano + "-" + numeroSeduta;
                    result.anno = dataSeduta.Year.ToString();
                    result.tipoRegistro = "Verbale sintetico di seduta";
                    break;
            }

            return result;
        }

        private InfoUtente getInfoUtenteVersamento(ItemsVersamento item)
        {
            InfoUtente retVal = new InfoUtente();
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            try
            {

                if (!(string.IsNullOrEmpty(item.idPeople) || string.IsNullOrEmpty(item.idGruppo)))
                {
                    Utente u = BusinessLogic.Utenti.UserManager.getUtenteById(item.idPeople);
                    Ruolo r = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(item.idGruppo);
                    retVal = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella ricostruzione dell'infoUtente autore del versamento: ", ex);
                retVal = null;
            }
            return retVal;
        }

        private DocsPaVO.ProfilazioneDinamica.OggettoCustom getContatoreRepertorio(SchedaDocumento doc)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom result = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();

            if (doc.template != null && doc.template.ELENCO_OGGETTI != null && doc.template.ELENCO_OGGETTI.Count > 0)
            {

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in doc.template.ELENCO_OGGETTI)
                {
                    if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore"))
                    {
                        result = ogg;
                        break;
                    }
                }
            }
            return result;
        }

        private string getRiferimentoTemporale(SchedaDocumento doc, InfoUtente utente, out string tipoRif)
        {
            string retVal = string.Empty;
            tipoRif = string.Empty;

            bool isAllegato = (doc.documentoPrincipale != null);
            if (isAllegato)
                logger.Debug("isAllegato");

            if (!isAllegato)
            {

                // 1 - data marca
                if (TimestampManager.getCountTimestampsDoc(utente, (DocsPaVO.documento.Documento)doc.documenti[0]) > 0)
                {
                    DocsPaVO.documento.TimestampDoc ts = (DocsPaVO.documento.TimestampDoc)TimestampManager.getTimestampsDoc(utente, (DocsPaVO.documento.Documento)doc.documenti[0])[0];
                    retVal = this.formatDateRifTemp(ts.DTA_CREAZIONE);
                    tipoRif = "Marca temporale";
                }
                else
                {
                    // 2 - data protocollazione
                    if (doc.protocollo != null && !string.IsNullOrEmpty(doc.protocollo.dataProtocollazione))
                    {
                        DocsPaDB.Query_DocsPAWS.Documenti d = new DocsPaDB.Query_DocsPAWS.Documenti();
                        retVal = this.formatDateRifTemp(d.GetRifTemporaleDocProtocollati(doc.systemId));
                        tipoRif = "Data di protocollazione";
                    }
                    else
                    {
                        // 3 - data repertoriazione
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = this.getContatoreRepertorio(doc);
                        if (contatore != null && contatore.CONS_REPERTORIO != null && contatore.CONS_REPERTORIO.Equals("1"))
                        {
                            retVal = this.formatDateRifTemp(contatore.DATA_INSERIMENTO);
                            tipoRif = "Data di repertoriazione";
                        }
                        /*
                        else
                        {
                            // 4 - data di creazione
                            retVal = this.formatDateRifTemp(doc.dataCreazione);
                            tipoRif = "Data di creazione";
                        }
                        */ 
                    }
                }
            }
            else
            {
                // GESTIONE ALLEGATI
                SchedaDocumento docPrincipale = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, doc.documentoPrincipale.docNumber);

                // 1 - data marca
                if (TimestampManager.getCountTimestampsDoc(utente, (DocsPaVO.documento.Documento)doc.documenti[0]) > 0)
                {
                    DocsPaVO.documento.TimestampDoc ts = (DocsPaVO.documento.TimestampDoc)TimestampManager.getTimestampsDoc(utente, (DocsPaVO.documento.Documento)doc.documenti[0])[0];
                    retVal = this.formatDateRifTemp(ts.DTA_CREAZIONE);
                    tipoRif = "Marca temporale";
                }
                else
                {
                    // 2 - data protocollazione documento principale
                    if (docPrincipale.protocollo != null && !string.IsNullOrEmpty(docPrincipale.protocollo.dataProtocollazione))
                    {
                        DocsPaDB.Query_DocsPAWS.Documenti d = new DocsPaDB.Query_DocsPAWS.Documenti();
                        retVal = this.formatDateRifTemp(d.GetRifTemporaleDocProtocollati(docPrincipale.systemId));
                        tipoRif = "Data di protocollazione";
                    }
                    else
                    {
                        // 3 - data repertoriazione documento principale
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom contatore = this.getContatoreRepertorio(docPrincipale);
                        if (contatore != null && contatore.CONS_REPERTORIO != null && contatore.CONS_REPERTORIO.Equals("1"))
                        {
                            retVal = this.formatDateRifTemp(contatore.DATA_INSERIMENTO);
                            tipoRif = "Data di repertoriazione";
                        }
                        /*
                        else
                        {
                            // 4 - data di creazione allegato
                            retVal = this.formatDateRifTemp(doc.dataCreazione);
                            tipoRif = "Data di creazione";
                        }
                        */
                    }
                }
            }

            // se il doc non è né marcato, né protocollato, né repertoriato utilizzo la data attuale
            if (string.IsNullOrEmpty(retVal))
            {
                retVal = this.formatDateRifTemp(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                tipoRif = "Data di versamento";
            }
            return retVal;

        }

        private ArrayList getFolderTree(Folder folder)
        {
            ArrayList tree = new ArrayList();
            Folder newFolder = folder;

            while (!(newFolder.idParent.Equals(newFolder.idFascicolo)))
            {
                tree.Add(newFolder);
                //newFolder = BusinessLogic.Fascicoli.FolderManager.getParentFolder(newFolder.systemID);
                newFolder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdNoSecurity(string.Empty, string.Empty, newFolder.idParent);
            }

            tree.Reverse();

            return tree;
        }

        private XmlDocument createXMLMetadati(SchedaDocumento doc, InfoUtente infoUt)
        {
            XmlDocument xml = new XmlDocument();
            logger.Debug("BEGIN");

            // reperimento dati su utente creatore doc
            DocsPaVO.utente.Utente utenteCreatore = BusinessLogic.Utenti.UserManager.getUtenteById(doc.creatoreDocumento.idPeople);
            DocsPaVO.utente.Ruolo ruoloCreatore = new DocsPaVO.utente.Ruolo();

            if (!string.IsNullOrEmpty(doc.creatoreDocumento.idCorrGlob_Ruolo))
                //ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuolo(doc.creatoreDocumento.idCorrGlob_Ruolo);
                ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(doc.creatoreDocumento.idCorrGlob_Ruolo);
            else
            {
                ArrayList ruoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(doc.creatoreDocumento.idPeople);
                if (ruoliUtente != null && ruoliUtente.Count > 0)
                    ruoloCreatore = (DocsPaVO.utente.Ruolo)ruoliUtente[0];

            }

            //InfoUtente infoUtente = Utenti.UserManager.GetInfoUtente(utenteCreatore, ruoloCreatore);
            InfoUtente infoUtente = infoUt;

            // caricamento template XML metadati
            xml.LoadXml(this.getTemplateXML(infoUtente.idAmministrazione, "M"));

            XmlElement elDocumento = (XmlElement)xml.SelectSingleNode("Documento");
            elDocumento.SetAttribute("IDdocumento", doc.docNumber);
            elDocumento.SetAttribute("DataCreazione", doc.dataCreazione);
            elDocumento.SetAttribute("Oggetto", doc.oggetto.descrizione);
            elDocumento.SetAttribute("Tipo", this.convertiTipoPoto(doc));
            if (doc.privato != null && doc.privato.Equals("1"))
                elDocumento.SetAttribute("LivelloRiservatezza", "Privato");

            XmlElement elAmm = (XmlElement)xml.SelectSingleNode("Documento/SoggettoProduttore/Amministrazione");
            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUt.idAmministrazione);
            elAmm.SetAttribute("CodiceAmministrazione", amm.Codice);
            elAmm.SetAttribute("DescrizioneAmministrazione", amm.Descrizione);

            // gerarchia UO
            logger.Debug("Gerarchia UO");
            if (!(ruoloCreatore != null))
            {
                logger.Debug("ruoloCreatore null!");
            }
            else
            {
                if (!(ruoloCreatore.uo != null))
                    logger.Debug("ruoloCreatore.uo null!");
            }
            //logger.Debug(string.Format("{0} - {1} - {2}", ruoloCreatore.codice, ruoloCreatore.descrizione, ruoloCreatore.uo.descrizione));
            if (ruoloCreatore != null && ruoloCreatore.uo != null)
            {
                int livello = Convert.ToInt32(ruoloCreatore.uo.livello);
                ArrayList gerarchiaUO = new ArrayList();
                gerarchiaUO.Add(ruoloCreatore.uo);
                if (livello > 1)
                {
                    UnitaOrganizzativa uo = ruoloCreatore.uo.parent;
                    for (int i = livello - 1; i > 0; i--)
                    {
                        UnitaOrganizzativa uoCorrente = new UnitaOrganizzativa();
                        uoCorrente = uo;
                        gerarchiaUO.Add(uoCorrente);
                        uo = uoCorrente.parent;
                    }
                    gerarchiaUO.Reverse();
                }
                string xpath = "Documento/SoggettoProduttore/GerarchiaUO";
                foreach (UnitaOrganizzativa uo in gerarchiaUO)
                {
                    XmlElement el = xml.CreateElement("UnitàOrganizzativa");
                    el.SetAttribute("CodiceUO", uo.codiceRubrica);
                    el.SetAttribute("DescrizioneUO", uo.descrizione);
                    el.SetAttribute("Livello", uo.livello);
                    xml.SelectSingleNode(xpath).AppendChild(el);
                    xpath = xpath + "/UnitàOrganizzativa";
                }
            }
            else
            {
                // rimuovo?
            }

            XmlElement elCreatore = (XmlElement)xml.SelectSingleNode("Documento/SoggettoProduttore/Creatore");
            if (ruoloCreatore != null)
            {
                elCreatore.SetAttribute("CodiceRuolo", ruoloCreatore.codiceRubrica);
                elCreatore.SetAttribute("DescrizioneRuolo", ruoloCreatore.descrizione);
            }
            if (utenteCreatore != null)
            {
                elCreatore.SetAttribute("CodiceUtente", utenteCreatore.userId);
                elCreatore.SetAttribute("DescrizioneUtente", utenteCreatore.descrizione);
            }
            // registrazione
            this.getRegistrazione(ref xml, doc, ruoloCreatore);

            // contesto archivistico
            this.getContestoArchivistico(ref xml, doc, ruoloCreatore, infoUtente);

            // tipologia
            logger.Debug("Tipologia");
            if (doc.template != null && doc.template.INVIO_CONSERVAZIONE != null && doc.template.INVIO_CONSERVAZIONE.Equals("1"))
            {
                XmlElement elTipologia = (XmlElement)xml.SelectSingleNode("Documento/Tipologia");
                elTipologia.SetAttribute("NomeTipologia", doc.template.DESCRIZIONE);

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in doc.template.ELENCO_OGGETTI)
                {
                    if (oggetto.CONSERVAZIONE != null && oggetto.CONSERVAZIONE.Equals("1"))
                    {
                        XmlElement element = xml.CreateElement("CampoTipologia");
                        element.SetAttribute("NomeCampo", oggetto.DESCRIZIONE);
                        //element.SetAttribute("ValoreCampo", oggetto.VALORE_DATABASE);
                        element.SetAttribute("ValoreCampo", this.getValoreOggettoCustom(oggetto));
                        elTipologia.AppendChild(element);
                    }
                }
            }
            else
            {
                xml.SelectSingleNode("Documento").RemoveChild(xml.SelectSingleNode("Documento/Tipologia"));
            }

            // allegati
            logger.Debug("Allegati");
            ArrayList allegati = BusinessLogic.Documenti.AllegatiManager.getAllegati(doc.docNumber, "all");
            if (allegati != null && allegati.Count > 0)
            {
                foreach (DocsPaVO.documento.Allegato all in allegati)
                {
                    XmlElement elAllegato = xml.CreateElement("Allegato");
                    switch (all.TypeAttachment)
                    {
                        case 1:
                            elAllegato.SetAttribute("Tipo", "Allegato Utente");
                            break;
                        case 2:
                            elAllegato.SetAttribute("Tipo", "Allegato PEC");
                            break;
                        case 3:
                            elAllegato.SetAttribute("Tipo", "Allegato Pitre");
                            break;
                        case 4:
                            elAllegato.SetAttribute("Tipo", "Allegato Altri Sistemi");
                            break;
                    }
                    elAllegato.SetAttribute("ID", all.docNumber);
                    elAllegato.SetAttribute("Descrizione", all.descrizione);

                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();

                    if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                    {
                        fdAll = BusinessLogic.Documenti.FileManager.getFile(all, infoUtente);
                        XmlElement elFile = this.addFileElement(ref xml, all, fdAll, infoUtente, true);
                        elAllegato.AppendChild(elFile);
                    }

                    xml.SelectSingleNode("Documento/Allegati").AppendChild(elAllegato);
                }

            }
            else
            {
                xml.SelectSingleNode("Documento").RemoveChild(xml.SelectSingleNode("Documento/Allegati"));
            }

            logger.Debug("Doc principale");
            DocsPaVO.documento.Documento docPrincipale = (DocsPaVO.documento.Documento)doc.documenti[0];
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFile(docPrincipale, infoUtente);
            XmlElement elemFile = this.addFileElement(ref xml, docPrincipale, fd, infoUtente, false);

            return xml;
        }

        private XmlElement addFileElement(ref XmlDocument xml, DocsPaVO.documento.FileRequest doc, DocsPaVO.documento.FileDocumento fileDoc, InfoUtente utente, bool add)
        {
            XmlElement elFile = null;

            if (add)
                elFile = xml.CreateElement("File");
            else
                elFile = (XmlElement)xml.SelectSingleNode("Documento/File");

            string impronta = this.getImpronta(doc);
            string algo = "N.A.";

            if (fileDoc != null)
            {
                if (impronta == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDoc.content))
                    algo = "SHA256";
                else if (impronta == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fileDoc.content))
                    algo = "SHA1";
            }

            elFile.SetAttribute("Formato", BusinessLogic.Documenti.FileManager.getExtFileFromPath(doc.fileName));
            elFile.SetAttribute("Dimensione", doc.fileSize);
            elFile.SetAttribute("Impronta", impronta);
            elFile.SetAttribute("AlgoritmoHash", algo);

            // 23-09-2015
            // I metadati su firma e marca non devono essere visualizzati nel file XML
            if (!add)
                elFile.RemoveChild(elFile.SelectSingleNode("MarcaTemporale"));

            #region CODICE COMMENTATO
            /*
            if (fileDoc != null && doc.firmato.Equals("1"))
            {
                elFile.AppendChild(this.EstraiDatiFirma(ref xml, fileDoc));
            }

            ArrayList timestampArray = TimestampManager.getTimestampsDoc(utente, doc);
            if (timestampArray != null && timestampArray.Count > 0)
            {
                DocsPaVO.documento.TimestampDoc ts = (DocsPaVO.documento.TimestampDoc)timestampArray[0];
                if (ts != null)
                {

                    XmlElement elMarca = null;
                    if (add)
                        elMarca = xml.CreateElement("MarcaTemporale");
                    else
                        elMarca = (XmlElement)xml.SelectSingleNode("Documento/File/MarcaTemporale");

                    elMarca.SetAttribute("NumeroSerie", ts.NUM_SERIE);
                    elMarca.SetAttribute("Data", this.getDate(ts.DTA_CREAZIONE));
                    elMarca.SetAttribute("Ora", this.getTime(ts.DTA_CREAZIONE));
                    elMarca.SetAttribute("SNCertificato", ts.S_N_CERTIFICATO);
                    elMarca.SetAttribute("DataInizioValidità", ts.DTA_CREAZIONE);
                    elMarca.SetAttribute("DataFineValidità", ts.DTA_SCADENZA);
                    elMarca.SetAttribute("ImprontaDocumentoAssociato", ts.TSR_FILE);
                    elMarca.SetAttribute("TimeStampingAuthority", ts.SOGGETTO);

                    if (add)
                        elFile.AppendChild(elMarca);
                }
            }
            else
            {
                if (!add)
                    elFile.RemoveChild(elFile.SelectSingleNode("MarcaTemporale"));
            }
            */

            #endregion

            return elFile;
        }

        private XmlElement EstraiDatiFirma(ref XmlDocument xml, DocsPaVO.documento.FileDocumento fileDoc)
        {
            XmlElement nodeFirma = xml.CreateElement("FirmaDigitale");

            if (fileDoc.signatureResult.PKCS7Documents != null)
            {

                foreach (PKCS7Document p7m in fileDoc.signatureResult.PKCS7Documents)
                {
                    if (p7m.SignersInfo != null)
                    {
                        DocsPaVO.documento.SignerInfo si = p7m.SignersInfo.FirstOrDefault();


                        XmlElement titolare = xml.CreateElement("Titolare");
                        XmlElement certificato = xml.CreateElement("Certificato");
                        XmlElement datiFirma = xml.CreateElement("DatiFirma");

                        titolare.SetAttribute("Nome", si.SubjectInfo.Nome);
                        titolare.SetAttribute("Cognome", si.SubjectInfo.Cognome);
                        titolare.SetAttribute("CodiceFiscale", si.SubjectInfo.CodiceFiscale);

                        certificato.InnerXml = string.Format("{0} {1} {2}", "<![CDATA[", this.SerializeObject<CertificateInfo>(si.CertificateInfo, true), "]]>");
                        datiFirma.InnerXml = string.Format("{0} {1} {2}", "<![CDATA[", this.SerializeObject<SubjectInfo>(si.SubjectInfo, true), "]]>");

                        nodeFirma.AppendChild(titolare);
                        nodeFirma.AppendChild(certificato);
                        nodeFirma.AppendChild(datiFirma);
                    }
                }
            }

            return nodeFirma;

        }

        public string convertiTipoPoto(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            string retval = schDoc.tipoProto;
            switch (schDoc.tipoProto)
            {

                case "A":
                case "P":
                case "I":
                    {
                        retval = "Protocollato";
                        if (schDoc.protocollo != null)
                            if (string.IsNullOrEmpty(schDoc.protocollo.segnatura))
                                retval = "Predisposto";
                    }
                    break;

                case "G":
                    retval = "Grigio";
                    break;
            }
            return retval;
        }

        private void getRegistrazione(ref XmlDocument xml, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Debug("BEGIN");
            XmlElement elRegistrazione = (XmlElement)xml.SelectSingleNode("Documento/Registrazione");

            if (schDoc.protocollo != null)
            {
                logger.Debug("dati protocollo");
                elRegistrazione.SetAttribute("DataProtocollo", this.getDate(schDoc.protocollo.dataProtocollazione));
                elRegistrazione.SetAttribute("OraProtocollo", this.getTime(schDoc.protocollo.dataProtocollazione));
                elRegistrazione.SetAttribute("NumeroProtocollo", schDoc.protocollo.numero);
                elRegistrazione.SetAttribute("SegnaturaProtocollo", schDoc.protocollo.segnatura);
                elRegistrazione.SetAttribute("TipoProtocollo", schDoc.tipoProto);

                if (schDoc.datiEmergenza != null)
                {
                    logger.Debug("dati emergenza");
                    elRegistrazione.SetAttribute("SegnaturaEmergenza", schDoc.datiEmergenza.protocolloEmergenza);
                    elRegistrazione.SetAttribute("NumeroProtocolloEmergenza", schDoc.datiEmergenza.protocolloEmergenza);
                    elRegistrazione.SetAttribute("DataProtocolloEmergenza", schDoc.datiEmergenza.dataProtocollazioneEmergenza);
                }
                else
                {
                    elRegistrazione.RemoveAttribute("SegnaturaEmergenza");
                    elRegistrazione.RemoveAttribute("NumeroProtocolloEmergenza");
                    elRegistrazione.RemoveAttribute("DataProtocolloEmergenza");
                }

                if (schDoc.registro != null)
                {
                    logger.Debug("dati registro");
                    elRegistrazione.SetAttribute("CodiceAOO", schDoc.registro.codRegistro);
                    elRegistrazione.SetAttribute("DescrizioneAOO", schDoc.registro.descrizione);
                }
                if (schDoc.protocollatore != null)
                {
                    logger.Debug("dati protocollatore");
                    XmlElement elProtocollista = (XmlElement)xml.SelectSingleNode("Documento/Registrazione/Protocollista");

                    DocsPaVO.utente.Utente userProt = BusinessLogic.Utenti.UserManager.getUtenteById(schDoc.protocollatore.utente_idPeople);
                    DocsPaVO.utente.Ruolo ruoloProt = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(schDoc.protocollatore.ruolo_idCorrGlobali);

                    if (userProt != null)
                    {
                        elProtocollista.SetAttribute("CodiceUtente", userProt.userId);
                        elProtocollista.SetAttribute("DescrizioneUtente", userProt.descrizione);
                    }
                    if (ruoloProt != null)
                    {
                        elProtocollista.SetAttribute("CodiceRuolo", ruoloProt.codiceRubrica);
                        elProtocollista.SetAttribute("DescrizioneRuolo", ruoloProt.descrizione);
                    }
                    if (ruolo != null && ruolo.uo != null)
                        elProtocollista.SetAttribute("UOAppartenenza", ruolo.uo.codiceRubrica);
                }
                else
                {
                    elRegistrazione.RemoveChild(elRegistrazione.SelectSingleNode("Protocollista"));
                }

                EstraiDatiProtoEntrata(schDoc, ref xml);
                EstraiDatiProtoUscita(schDoc, ref xml);
                EstraiDatiProtoInterno(schDoc, ref xml);

            }
            else
            {
                xml.SelectSingleNode("Documento").RemoveChild(elRegistrazione);
            }

            logger.Debug("END");

        }

        private void EstraiDatiProtoEntrata(DocsPaVO.documento.SchedaDocumento schDoc, ref XmlDocument xml)
        {

            logger.Debug("BEGIN");
            DocsPaVO.documento.ProtocolloEntrata protEnt = schDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata;

            if (protEnt != null)
            {
                XmlElement protoMittente = (XmlElement)xml.SelectSingleNode("Documento/Registrazione/ProtocolloMittente");
                protoMittente.SetAttribute("Protocollo", protEnt.descrizioneProtocolloMittente);
                protoMittente.SetAttribute("Data", protEnt.dataProtocolloMittente);
                protoMittente.SetAttribute("MezzoSpedizione", protEnt.descMezzoSpedizione);

                DocsPaVO.utente.Corrispondente corr = protEnt.mittente;

                if (protEnt.mittenti != null)
                {
                    foreach (DocsPaVO.utente.Corrispondente c in protEnt.mittenti)
                    {
                        XmlElement mittente = xml.CreateElement("Mittente");
                        mittente.SetAttribute("Codice", c.codiceRubrica);
                        mittente.SetAttribute("Descrizione", c.descrizione);
                        mittente.SetAttribute("ProtocolloMittente", protEnt.numero);
                        mittente.SetAttribute("DataProtocolloMittente", protEnt.dataProtocolloMittente);
                        mittente.SetAttribute("IndirizzoMail", c.email);
                        xml.SelectSingleNode("Documento/Registrazione").AppendChild(mittente);
                    }
                }

                if (protEnt.mittenteIntermedio != null)
                {
                    XmlElement mittente = xml.CreateElement("Mittente");
                    mittente.SetAttribute("Codice", protEnt.mittenteIntermedio.codiceRubrica);
                    mittente.SetAttribute("Descrizione", protEnt.mittenteIntermedio.descrizione);
                    mittente.SetAttribute("ProtocolloMittente", protEnt.numero);
                    mittente.SetAttribute("DataProtocolloMittente", protEnt.dataProtocolloMittente);
                    mittente.SetAttribute("IndirizzoMail", protEnt.mittenteIntermedio.email);
                    xml.SelectSingleNode("Documento/Registrazione").AppendChild(mittente);

                }

            }
        }

        private void EstraiDatiProtoUscita(DocsPaVO.documento.SchedaDocumento schDoc, ref XmlDocument xml)
        {
            logger.Debug("BEGIN");

            DocsPaVO.documento.ProtocolloUscita protUsc = schDoc.protocollo as DocsPaVO.documento.ProtocolloUscita;
            if (protUsc != null)
            {

                if (protUsc.mittente != null)
                {
                    XmlElement mittente = xml.CreateElement("Mittente");
                    mittente.SetAttribute("Codice", protUsc.mittente.codiceRubrica);
                    mittente.SetAttribute("Descrizione", protUsc.mittente.descrizione);
                    mittente.SetAttribute("ProtocolloMittente", string.Empty);
                    mittente.SetAttribute("DataProtocolloMittente", string.Empty);
                    mittente.SetAttribute("IndirizzoMail", protUsc.mittente.email);
                    xml.SelectSingleNode("Documento/Registrazione").AppendChild(mittente);
                }

                if (protUsc.destinatari != null)
                {
                    foreach (DocsPaVO.utente.Corrispondente c in protUsc.destinatari)
                    {
                        XmlElement dest = xml.CreateElement("Destinatario");
                        dest.SetAttribute("Codice", c.codiceRubrica);
                        dest.SetAttribute("Descrizione", c.descrizione);
                        dest.SetAttribute("MezzoSpedizione", protUsc.mezzoSpedizione.ToString());
                        dest.SetAttribute("IndirizzoMail", c.email);
                        xml.SelectSingleNode("Documento/Registrazione").AppendChild(dest);
                    }
                }
                if (protUsc.destinatariConoscenza != null)
                {
                    foreach (DocsPaVO.utente.Corrispondente c in protUsc.destinatariConoscenza)
                    {

                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        XmlElement dest = xml.CreateElement("Destinatario");
                        dest.SetAttribute("Codice", c.codiceRubrica);
                        dest.SetAttribute("Descrizione", c.descrizione);
                        dest.SetAttribute("MezzoSpedizione", protUsc.mezzoSpedizione.ToString());
                        dest.SetAttribute("IndirizzoMail", c.email);
                        xml.SelectSingleNode("Documento/Registrazione").AppendChild(dest);
                    }
                }
            }
        }

        private void EstraiDatiProtoInterno(DocsPaVO.documento.SchedaDocumento schDoc, ref XmlDocument xml)
        {

            logger.Debug("BEGIN");
            DocsPaVO.documento.ProtocolloInterno protInt = schDoc.protocollo as DocsPaVO.documento.ProtocolloInterno;


            if (protInt != null)
            {
                if (protInt.mittente != null)
                {
                    XmlElement mittente = xml.CreateElement("Mittente");
                    mittente.SetAttribute("Codice", protInt.mittente.codiceRubrica);
                    mittente.SetAttribute("Descrizione", protInt.mittente.descrizione);
                    mittente.SetAttribute("ProtocolloMittente", string.Empty);
                    mittente.SetAttribute("DataProtocolloMittente", string.Empty);
                    mittente.SetAttribute("IndirizzoMail", protInt.mittente.email);
                    xml.SelectSingleNode("Documento/Registrazione").AppendChild(mittente);
                }

                if (protInt.destinatari != null)
                {
                    foreach (DocsPaVO.utente.Corrispondente c in protInt.destinatari)
                    {
                        XmlElement dest = xml.CreateElement("Destinatario");
                        dest.SetAttribute("Codice", c.codiceRubrica);
                        dest.SetAttribute("Descrizione", c.descrizione);
                        dest.SetAttribute("MezzoSpedizione", protInt.mezzoSpedizione.ToString());
                        dest.SetAttribute("IndirizzoMail", c.email);
                        xml.SelectSingleNode("Documento/Registrazione").AppendChild(dest);
                    }
                }
                if (protInt.destinatariConoscenza != null)
                {
                    foreach (DocsPaVO.utente.Corrispondente c in protInt.destinatariConoscenza)
                    {
                        XmlElement dest = xml.CreateElement("Destinatario");
                        dest.SetAttribute("Codice", c.codiceRubrica);
                        dest.SetAttribute("Descrizione", c.descrizione);
                        dest.SetAttribute("MezzoSpedizione", protInt.mezzoSpedizione.ToString());
                        dest.SetAttribute("IndirizzoMail", c.email);
                        xml.SelectSingleNode("Documento/Registrazione").AppendChild(dest);
                    }
                }

            }
        }

        private void getContestoArchivistico(ref XmlDocument xml, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {

            logger.Debug("BEGIN");

            object[] fasAList = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, schDoc.systemId).ToArray();
            foreach (DocsPaVO.fascicolazione.Fascicolo fas in fasAList)
            {
                if (fas != null)
                {
                    if (!(fas.tipo == "P"))
                    {
                        DocsPaVO.amministrazione.OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(fas.idTitolario);

                        XmlElement elClass = (XmlElement)xml.SelectSingleNode("Documento/ContestoArchivistico/Classificazione");
                        //elClass.SetAttribute("CodiceClassificazione", nodo.Codice);
                        elClass.SetAttribute("CodiceClassificazione", fas.codice);
                        elClass.SetAttribute("TitolarioDiRiferimento", string.Format("{0} - attivo al {1}", nodo.Descrizione, DateTime.Now.ToString("dd/MM/yyyy")));

                    }
                    else
                    {
                        XmlElement element = xml.CreateElement("Fascicolazione");
                        element.SetAttribute("CodiceFascicolo", fas.codice);
                        element.SetAttribute("DescrizioneFascicolo", fas.descrizione);
                        string titolario = string.Empty;
                        if (fas.idTitolario != null)
                        {
                            DocsPaVO.amministrazione.OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(fas.idTitolario);
                            element.SetAttribute("TitolarioDiRiferimento", string.Format("{0} - attivo al {1}", nodo.Descrizione, DateTime.Now.ToString("dd/MM/yyyy")));
                            titolario = string.Format("{0} - attivo al {1}", nodo.Descrizione, DateTime.Now.ToString("dd/MM/yyyy"));
                        }
                        xml.SelectSingleNode("Documento/ContestoArchivistico").AppendChild(element);

                        foreach (DocsPaVO.fascicolazione.Folder f in BusinessLogic.Fascicoli.FolderManager.GetFoldersDocument(schDoc.systemId, fas.systemID))
                        {
                            XmlElement subFasc = xml.CreateElement("Fascicolazione");
                            subFasc.SetAttribute("CodiceFascicolo", fas.codice);
                            subFasc.SetAttribute("DescrizioneFascicolo", fas.descrizione);
                            subFasc.SetAttribute("CodiceSottoFascicolo", f.systemID);
                            subFasc.SetAttribute("DescrizioneSottoFascicolo", f.descrizione);
                            subFasc.SetAttribute("TitolarioDiRiferimento", titolario);
                            xml.SelectSingleNode("Documento/ContestoArchivistico").AppendChild(subFasc);
                        }
                    }
                }
            }

            logger.Debug("DOC COLLEGATI");
            if (schDoc.rispostaDocumento != null && (!string.IsNullOrEmpty(schDoc.rispostaDocumento.segnatura) || !string.IsNullOrEmpty(schDoc.rispostaDocumento.docNumber)))
            {
                if ((schDoc.rispostaDocumento.docNumber != null) && (schDoc.rispostaDocumento.idProfile != null))
                {
                    //SchedaDocumento sc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schDoc.rispostaDocumento.idProfile, schDoc.rispostaDocumento.docNumber);
                    SchedaDocumento sc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, schDoc.rispostaDocumento.idProfile);

                    if (sc != null)
                    {
                    XmlElement element = (XmlElement)xml.SelectSingleNode("Documento/ContestoArchivistico/DocumentoCollegato");
                    element.SetAttribute("IDdocumento", schDoc.rispostaDocumento.idProfile);
                    element.SetAttribute("DataCreazione", this.getDate(schDoc.dataCreazione));
                    element.SetAttribute("Oggetto", sc.oggetto.descrizione);

                    if (sc.protocollo != null)
                    {
                        element.SetAttribute("DataProtocollo", this.getDate(sc.protocollo.dataProtocollazione));
                        element.SetAttribute("NumeroProtocollo", sc.protocollo.numero);
                        element.SetAttribute("SegnaturaProtocollo", sc.protocollo.segnatura);
                    }
                }
            }
            }
            else
            {
                xml.SelectSingleNode("Documento/ContestoArchivistico").RemoveChild(xml.SelectSingleNode("Documento/ContestoArchivistico/DocumentoCollegato"));
            }

            logger.Debug("END");
        }

        public String SerializeObject<t>(Object pObject, bool nsNull)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                if (nsNull)
                {
                    ns.Add("", "");
                    xs.Serialize(xmlTextWriter, pObject, ns);
                }
                else
                {
                    ns.Add("sincro", "http://www.cnipa.gov.it/sincro/");
                    xs.Serialize(xmlTextWriter, pObject, ns);
                }
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }

        /// <summary>
        /// Sostituisce i caratteri speciali ISO-8859-1 oltre l'indice 127 con il corrispondente bytecode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string replaceSpecialChars(string text)
        {
            string result = text;

            #region Caratteri speciali (cod. 160-191)

            result = result.Replace("`", "&#096;");
            result = result.Replace("¡", "&#161;");
            result = result.Replace("¢", "&#162;");
            result = result.Replace("£", "&#163;");
            result = result.Replace("¤", "&#164;");
            result = result.Replace("¥", "&#165;");
            result = result.Replace("¦", "&#166;");
            result = result.Replace("§", "&#167;");
            result = result.Replace("¨", "&#168;");
            result = result.Replace("©", "&#169;");
            result = result.Replace("ª", "&#170;");
            result = result.Replace("«", "&#171;");
            result = result.Replace("¬", "&#172;");
            result = result.Replace("®", "&#174;");
            result = result.Replace("¯", "&#175;");
            result = result.Replace("°", "&#176;");
            result = result.Replace("±", "&#177;");
            result = result.Replace("²", "&#178;");
            result = result.Replace("³", "&#179;");
            result = result.Replace("´", "&#180;");
            result = result.Replace("µ", "&#181;");
            result = result.Replace("¶", "&#182;");
            result = result.Replace("·", "&#183;");
            result = result.Replace("¸", "&#184;");
            result = result.Replace("¹", "&#185;");
            result = result.Replace("º", "&#186;");
            result = result.Replace("»", "&#187;");
            result = result.Replace("¼", "&#188;");
            result = result.Replace("½", "&#189;");
            result = result.Replace("¾", "&#190;");
            result = result.Replace("¿", "&#191;");

            #endregion

            #region Lettere accentate e simili (cod. 192-255)

            result = result.Replace("À", "&#192;");
            result = result.Replace("Á", "&#193;");
            result = result.Replace("Â", "&#194;");
            result = result.Replace("Ã", "&#195;");
            result = result.Replace("Ä", "&#196;");
            result = result.Replace("Å", "&#197;");
            result = result.Replace("Æ", "&#198;");
            result = result.Replace("Ç", "&#199;");
            result = result.Replace("È", "&#200;");
            result = result.Replace("É", "&#201;");
            result = result.Replace("Ê", "&#202;");
            result = result.Replace("Ë", "&#203;");
            result = result.Replace("Ì", "&#204;");
            result = result.Replace("Í", "&#205;");
            result = result.Replace("Î", "&#206;");
            result = result.Replace("Ï", "&#207;");
            result = result.Replace("Ð", "&#208;");
            result = result.Replace("Ñ", "&#209;");
            result = result.Replace("Ò", "&#210;");
            result = result.Replace("Ó", "&#211;");
            result = result.Replace("Ô", "&#212;");
            result = result.Replace("Õ", "&#213;");
            result = result.Replace("Ö", "&#214;");
            result = result.Replace("×", "&#215;");
            result = result.Replace("Ø", "&#216;");
            result = result.Replace("Ù", "&#217;");
            result = result.Replace("Ú", "&#218;");
            result = result.Replace("Û", "&#219;");
            result = result.Replace("Ü", "&#220;");
            result = result.Replace("Ý", "&#221;");
            result = result.Replace("Þ", "&#222;");
            result = result.Replace("ß", "&#223;");
            result = result.Replace("à", "&#224;");
            result = result.Replace("á", "&#225;");
            result = result.Replace("â", "&#226;");
            result = result.Replace("ã", "&#227;");
            result = result.Replace("ä", "&#228;");
            result = result.Replace("å", "&#229;");
            result = result.Replace("æ", "&#230;");
            result = result.Replace("ç", "&#231;");
            result = result.Replace("è", "&#232;");
            result = result.Replace("é", "&#233;");
            result = result.Replace("ê", "&#234;");
            result = result.Replace("ë", "&#235;");
            result = result.Replace("ì", "&#236;");
            result = result.Replace("í", "&#237;");
            result = result.Replace("í", "&#238;");
            result = result.Replace("í", "&#239;");
            result = result.Replace("í", "&#240;");
            result = result.Replace("í", "&#241;");
            result = result.Replace("ò", "&#242;");
            result = result.Replace("ó", "&#243;");
            result = result.Replace("ô", "&#244;");
            result = result.Replace("õ", "&#245;");
            result = result.Replace("ö", "&#246;");
            result = result.Replace("÷", "&#247;");
            result = result.Replace("ø", "&#248;");
            result = result.Replace("ù", "&#249;");
            result = result.Replace("ú", "&#250;");
            result = result.Replace("û", "&#251;");
            result = result.Replace("ü", "&#252;");
            result = result.Replace("ý", "&#253;");
            result = result.Replace("þ", "&#254;");
            result = result.Replace("ÿ", "&#255;");

            #endregion

            return result;

        }

        /// <summary>
        /// Sostituisce i caratteri non ammessi per ente, struttura e registro con valori ammessi
        /// I valori ammessi sono lettere maiuscole e minuscole, numeri, underscore ("_"), punto ("."), spazio (" ") e trattino ("-")
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string replaceSpecialCharsHeader(string text)
        {
            string result = text;

            result = result.Replace("/", "-");
            result = result.Replace("\\", "-");
            result = result.Replace("|", "-");
            result = result.Replace("(", "-");
            result = result.Replace(")", "-");

            result = result.Replace(",", " ");
            result = result.Replace(";", " ");
            result = result.Replace(":", " ");
            result = result.Replace("'", " ");

            result = result.Replace("À", "A");
            result = result.Replace("Á", "A");
            result = result.Replace("È", "E");
            result = result.Replace("É", "E");
            result = result.Replace("É", "E");
            result = result.Replace("Ì", "I");
            result = result.Replace("Í", "I");
            result = result.Replace("Ò", "O");
            result = result.Replace("Ó", "O");
            result = result.Replace("Ù", "U");
            result = result.Replace("Ú", "U");

            result = result.Replace("à", "a");
            result = result.Replace("á", "a");
            result = result.Replace("è", "e");
            result = result.Replace("é", "e");
            result = result.Replace("ì", "i");
            result = result.Replace("í", "i");
            result = result.Replace("ò", "o");
            result = result.Replace("ó", "o");
            result = result.Replace("ù", "u");
            result = result.Replace("ú", "u");

            return result;
        }

        /// <summary>
        /// Trasforma la data nel formato YYYY-MM-DD
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string formatDate(string date)
        {
            logger.Debug("format date " + date);
            string result = this.getDate(date);
            string d = result.Split('/')[0];
            string m = result.Split('/')[1];
            string y = result.Split('/')[2];

            result = y + "-" + m + "-" + d;
            return result;
        }

        private string formatTime(string time)
        {
            string result = time.Replace('.', ':');
            return result;
        }

        /// <summary>
        /// Trasforma la data/ora nel formato YYYY-MM-DDTHH:MM:SS.mms+HH:HH
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private string formatDateRifTemp(string datetime)
        {

            string result = string.Empty;

            try
            {
                result = (Convert.ToDateTime(datetime)).ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }

            return result;
        }

        /// <summary>
        /// Restituisce l'estensione del file.
        /// In caso di file firmati restituisce estensione originale.p7m
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string getExtension(string filename)
        {
            string retVal = string.Empty;
            string ext = BusinessLogic.Documenti.FileManager.getExtFileFromPath(filename);
            if (ext.ToUpper().Equals("P7M"))
            {
                retVal = BusinessLogic.Documenti.FileManager.getEstensioneIntoSignedFile(filename) + ".p7m";
            }
            else
            {
                retVal = ext;
            }

            return retVal;
        }

        private string GetNomeOriginale(InfoUtente u, DocsPaVO.documento.FileRequest fr)
        {
            string result = string.Empty;

            try
            {
                result = BusinessLogic.Documenti.FileManager.getOriginalFileName(u, fr);
                if (string.IsNullOrEmpty(result))
                {
                    if (!string.IsNullOrEmpty(fr.fileName))
                        result = fr.fileName;
                    else
                        result = fr.versionId;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(fr.fileName))
                    result = fr.fileName;
                else
                    result = fr.versionId;
            }

            return result;
        }

        public DocsPaVO.areaConservazione.StampaRegistro getInfoStampaRegistro(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getInfoStampaRegistro(idDoc);
        }

        public DocsPaVO.areaConservazione.StampaRegistro getInfoStampaRepertorio(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getInfoStampaReperiorio(idDoc);
        }

        private string getDataRepertoriazione(string anno, string value, string idOggetto)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getDataRepertoriazione(anno, value, idOggetto);
        }

        private string getVersioneDatiSpecifici(string idAmm, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getVersioneDatiSpecifici(idAmm, tipo);
        }

        private string getNomeTipologia(string idContatore)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getNomeTipologia(idContatore);
        }

        public string getNomeTipologiaDaDoc(string idDocRepertoriato)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getNomeTipologiaDaDoc(idDocRepertoriato);
        }

        public string getNomeTipologiaDaStampa(string idDocStampa)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getNomeTipologiaDaStampa(idDocStampa);
        }

        private string getValoreOggettoCustom(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            string riga = string.Empty;

            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "Corrispondente":
                    DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(oggettoCustom.VALORE_DATABASE);
                    if (corr != null && corr.descrizione != null)
                        riga += corr.descrizione;
                    break;

                case "Contatore":
                    string contatore = string.Empty;
                    if (oggettoCustom.FORMATO_CONTATORE != "")
                    {
                        contatore = oggettoCustom.FORMATO_CONTATORE;
                        if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                        {
                            contatore = contatore.Replace("ANNO", oggettoCustom.ANNO);
                            contatore = contatore.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    contatore = contatore.Replace("RF", reg.codRegistro);
                                    contatore = contatore.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            contatore = string.Empty;
                        }
                    }
                    else
                    {
                        contatore = oggettoCustom.VALORE_DATABASE;
                    }

                    riga += contatore;
                    break;

                case "CasellaDiSelezione":
                    string selezione = string.Empty;
                    foreach (string sel in oggettoCustom.VALORI_SELEZIONATI)
                    {
                        if (sel != null && sel != "")
                            selezione += sel + " - ";
                    }
                    if (selezione != null && selezione != string.Empty)
                        riga += selezione.Substring(0, selezione.Length - 2);
                    break;

                case "ContatoreSottocontatore":
                    string s_contatore = string.Empty;
                    if (oggettoCustom.FORMATO_CONTATORE != "")
                    {
                        s_contatore = oggettoCustom.FORMATO_CONTATORE;
                        if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                        {
                            s_contatore = s_contatore.Replace("ANNO", oggettoCustom.ANNO);
                            s_contatore = s_contatore.Replace("CONTATORE", (oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE));
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    s_contatore = s_contatore.Replace("RF", reg.codRegistro);
                                    s_contatore = s_contatore.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            s_contatore = string.Empty;
                        }
                    }
                    else
                    {
                        s_contatore = oggettoCustom.VALORE_DATABASE;
                    }

                    riga += s_contatore;
                    break;


                default:
                    riga += oggettoCustom.VALORE_DATABASE;
                    break;

            }

            return riga;

        }

        private string getConfigKey(string idAmm, string key)
        {
            string retVal = string.Empty;

            // se è definita una chiave globale restituisco questa...
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", key)))
            {
                retVal = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", key);
            }
            else
            // ...altrimenti verifico l'esistenza di una chiave definita per l'amministrazione
            {
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key)))
                {
                    retVal = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key);
                }
                else
                {
                    retVal = string.Empty;
                }
            }

            return retVal;

        }

        public string GetIdFascPrimaFascicolazione(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione manager = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return manager.GetIdFascPrimaFascicolazione(idDoc);
        }

        public string GetIdRuoloRespConservazione(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione manager = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return manager.GetIdRoleResponsabileConservazione(idAmm);
        }

        public string GetIdUtenteRespConservazione(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione manager = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return manager.GetIdUtenteResponsabileConservazione(idAmm);
        }



        /// <summary>
        /// Estende la visibilità sul documento al ruolo responsabile della conservazione
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="idGruppo">id gruppo ruolo responsabile</param>
        /// <param name="idVersatore">id ruolo autore del versamento</param>
        public void SetVisibilitaRuoloResp(string docnumber, string idGruppo, string idVersatore)
        {

            logger.Debug("BEGIN");

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            // Verifico se il ruolo responsabile ha già visibilità sul documento
            string checksecurity = cons.VerificaDirittiInSecurity(docnumber, idGruppo);
            if (!string.IsNullOrEmpty(checksecurity))
            {
                if (checksecurity.Equals("OK"))
                {
                    // il ruolo ha già diritti di lettura/scrittura sul documento
                    logger.Debug("diritti già presenti");
                }
                else
                {
                    bool result = false;
                    if (checksecurity.Equals("INSERT"))
                        result = cons.insertInSecurity(docnumber, idGruppo, idVersatore);
                    if (checksecurity.Equals("UPDATE"))
                        result = cons.UpdateSecurityConservazione(docnumber, idGruppo);

                    if (result)
                    {
                        // Aggiornamento documentale
                    }

                }
            }
            
            }
            
        public bool SaveErrorMessage(string idProfile, string error)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.SaveErrorMessage(idProfile, error);
        }

        public bool CtrlXMLFattura(DocsPaVO.documento.FileRequest fr, InfoUtente infoUt, SchedaDocumento doc)
        {
            logger.Debug("Controllo XML Fattura ID=" + fr.versionId);
            bool retval = false;
            try
            {
                string valoreInXml = "";
                string mappingNumFattura = "//*[name()='FatturaElettronicaBody']/*[name()='DatiGenerali']/*[name()='DatiGeneraliDocumento']/*[name()='Numero']";
                if (fr.fileName.ToUpper().EndsWith("XML") || fr.fileName.ToUpper().EndsWith("XML.P7M"))
                {
                    FileDocumento file1 = BusinessLogic.Documenti.FileManager.getFile(fr, infoUt);
                    string stringaXml = Encoding.UTF8.GetString(file1.content);
                    //string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    //if (stringaXml.StartsWith(byteOrderMarkUtf8))
                    //{
                    //    logger.Debug("byteOrderMarkUtf8 presente, lo rimuovo");
                    //    stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                    //}
                    stringaXml = stringaXml.Trim();
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    if (stringaXml.Contains("xml version=\"1.1\""))
                    {
                        logger.Debug("Versione XML 1.1. Provo conversione");
                        stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                    }

                    try
                    {
                        xmlDoc.LoadXml(stringaXml);
                    }
                    catch (Exception bomUTF8)
                    {
                        logger.Debug("Errore bomUTF8");
                        string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (stringaXml.StartsWith(byteOrderMarkUtf8))
                        {
                            stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                        }
                        xmlDoc.LoadXml(stringaXml);
                    }
                    logger.Debug("Stringa caricata in XML");
                    if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in doc.template.ELENCO_OGGETTI)
                        {
                            if (ogg.DESCRIZIONE.ToUpper() == "NUMERO FATTURA")
                            {
                                valoreInXml = xmlDoc.DocumentElement.SelectSingleNode(mappingNumFattura).InnerXml;
                                // Estrazione dei campi CDATA
                                if (valoreInXml.Contains("<![CDATA["))
                                {
                                    valoreInXml = valoreInXml.Replace("<![CDATA[", "");
                                    valoreInXml = valoreInXml.Replace("]]>", "");
                                }
                                logger.DebugFormat("Num fattura. In DB {0}. In XML {1}.", ogg.VALORE_DATABASE, valoreInXml);
                                if (ogg.VALORE_DATABASE.Trim() == valoreInXml.Trim())
                                    retval = true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex3)
            {
                logger.Error(ex3.Message);
                retval = false;
            }
            logger.Debug("END - " + retval.ToString());
            return retval;
        }

        public string GetCodiceAOO(string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.GetCodiceAOO(idProfile);
        }

        #endregion
    }


}
