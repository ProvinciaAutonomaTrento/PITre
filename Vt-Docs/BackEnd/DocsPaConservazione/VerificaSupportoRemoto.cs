using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using log4net;
using DocsPaVO.areaConservazione;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class StatoVerificaSupporto
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Esito
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime InizioVerifica
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime FineVerifica
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Percentuale
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int DocumentiVerificati
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int DocumentiNonValidi
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotaleDocumenti
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Classe per la gestione della logica applicativa legata ai supporti
    /// </summary>
    public class VerificaSupportoRemoto
    {
        /// <summary>
        /// 
        /// </summary>
        private static ILog logger = LogManager.GetLogger(typeof(VerificaSupportoRemoto));

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, VerificaSupportoRemoto> _command = new Dictionary<string, VerificaSupportoRemoto>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        private VerificaSupportoRemoto(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            this._infoUtente = infoUtente;
            this._idConservazione = idConservazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        private static string GetDictKey(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            return string.Format("{0}_{1}", infoUtente.idPeople, idConservazione);
        }

        /// <summary>
        /// 
        /// </summary>
        private delegate void IniziaVerificaSupportoDelegate();

        /// <summary>
        /// Esecuzione del task di verifica integrità del supporto remoto
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        public static void IniziaVerifica(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            string key = GetDictKey(infoUtente, idConservazione);

            if (!_command.ContainsKey(key))
            {
                VerificaSupportoRemoto cmd = new VerificaSupportoRemoto(infoUtente, idConservazione);

                _command.Add(key, cmd);

                // Avvio task di verifica asincrono
                IniziaVerificaSupportoDelegate del = new IniziaVerificaSupportoDelegate(cmd.IniziaVerificaSupporto);
                IAsyncResult result = del.BeginInvoke(new AsyncCallback(cmd.IniziaVerificaSupportoExecuted), cmd);
            }
            else
            {
                // Task di verifica già iniziato
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void IniziaVerificaSupportoExecuted(IAsyncResult result)
        {
            //string esito ="0";
            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;

            if (result.IsCompleted)
            {
                VerificaSupportoRemoto cmd = (VerificaSupportoRemoto)result.AsyncState;

                // Notifica trasmissione al mittente
                DocsPaConsManager consManager = new DocsPaConsManager();

                if (cmd._statoVerifica.Esito)
                {
                    bool ret = consManager.TrasmettiNotifica(this._infoUtente, this._idConservazione);
                    //logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                    //esito = "1";

                }



            }
        }

        protected string FetchDataProssimaVerifica()
        {
            // Se l'istanza è stata creata tramite policy, riporta il numero dei mesi definiti nella stessa, altrimenti il default è 6 mesi
            // MEV CS 1.5
            // il valore di default è quello della chiave di db BE_CONSERVAZIONE_INTERVALLO

            DocsPaConsManager dcmgr = new DocsPaConsManager();
            DocsPaVO.Conservazione.Policy policy = dcmgr.GetPolicyByIdIstanzaConservazione(this._infoUtente, this._idConservazione);
            //int mesiDefault = 6;

            // recupero il valore dal db (espresso in GIORNI)
            int giorniVerifica = Convert.ToInt32(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(this._infoUtente.idAmministrazione, "BE_CONSERVAZIONE_INTERVALLO"));

            if (policy != null)
            {
                // Istanza creata tramite policy: impostazione del numero di mesi definiti in essa
                int mesiPolicy;
                Int32.TryParse(policy.avvisoMesi, out mesiPolicy);
                //if (mesiPolicy == 0) 
                //    mesiPolicy = mesiDefault;
                //return DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");

                // MEV CS 1.5
                if (mesiPolicy == 0)
                {
                    return DateTime.Today.AddDays(giorniVerifica).ToString("dd/MM/yyyy");
                }
                else
                {
                    return DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");
                }
            }
            else
            {
                // Default: prossimo controllo a 6 mesi da oggi
                // return DateTime.Today.AddMonths(mesiDefault).ToString("dd/MM/yyyy");

                // MEV CS 1.5
                // restituisco il valore calcolato a partire dall'intervallo definito nel db
                return DateTime.Today.AddDays(giorniVerifica).ToString("dd/MM/yyyy");
            }
        }

        protected string FetchIdSupporto()
        {
            string filters = " A.ID_CONSERVAZIONE= '" + this._idConservazione + "'  AND  ( A.ID_TIPO_SUPPORTO = (SELECT system_id from dpa_tipo_supporto where var_tipo='REMOTO') ) and a.ID_CONSERVAZIONE = c.SYSTEM_ID and c.id_amm = " + this._infoUtente.idAmministrazione + " ORDER BY A.ID_CONSERVAZIONE DESC, A.COPIA ASC";
            DocsPaConsManager dcmgr = new DocsPaConsManager();
            DocsPaVO.areaConservazione.InfoSupporto[] supporti = dcmgr.RicercaInfoSupporto(filters);
            return supporti.FirstOrDefault().SystemID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        private static void EliminaVerifica(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            string key = GetDictKey(infoUtente, idConservazione);

            if (_command.ContainsKey(key))
            {
                VerificaSupportoRemoto cmd = new VerificaSupportoRemoto(infoUtente, idConservazione);

                if (cmd._statoVerifica.Percentuale == 100)
                {
                    _command.Remove(key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public static StatoVerificaSupporto GetStatoVerifica(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            StatoVerificaSupporto stato = null;

            string key = GetDictKey(infoUtente, idConservazione);

            if (_command.ContainsKey(key))
            {
                VerificaSupportoRemoto itm = _command[key];

                stato = itm._statoVerifica;
            }

            return stato;
        }


        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private string _idConservazione = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private bool _interrompi = false;

        /// <summary>
        /// 
        /// </summary>
        private StatoVerificaSupporto _statoVerifica = new StatoVerificaSupporto();

        /// <summary>
        /// 
        /// </summary>
        protected virtual void IniziaVerificaSupporto()
        {
            logger.Debug("BEGIN - IniziaVerificaSupporto");
            bool hasError = false;
            string errorMessage = string.Empty;

            string dctmCServerAddressRoot = string.Empty;

            //Per rendere asincrona l'operazione di spostamento dei file Impostare a true 
            //Questa operazione è da fare pure su esitoLeggibilita in Docspaconservazionews.asmx
            bool operazioneAsincrona = true;

            if (operazioneAsincrona)
            {
                FileManager fm = new FileManager();
                fm.createZipFile(this._idConservazione);
                if (!fm.SubmitToRemoteFolder(this._idConservazione))
                {
                    logger.ErrorFormat("Errore inviando i file sullo starge remoto");
                    hasError = true;
                    return;
                }
                else
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(this._infoUtente,
                              "CREAZIONE_STORAGE",
                              this._idConservazione,
                              String.Format("Memorizzazione nel repository del sistema di conservazione dell’istanza  {0}", this._idConservazione),
                              DocsPaVO.Logger.CodAzione.Esito.OK);

                    // Modifica scrittura Log e Registro di Conservazione per la scrittura della Creazione Storage
                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                    regCons.idAmm = this._infoUtente.idAmministrazione;
                    regCons.idIstanza = this._idConservazione;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = this._infoUtente.userId;
                    regCons.codAzione = "CREAZIONE_STORAGE";
                    regCons.descAzione = "Memorizzazione nel repository del sistema di conservazione dell'istanza " + this._idConservazione;
                    regCons.esito = "1";
                    RegistroConservazione rc = new RegistroConservazione();
                    rc.inserimentoInRegistroCons(regCons, this._infoUtente);
                }
            }

            try
            {
                // Impostazione del supporto in stato "InVerifica"
                DocsPaConsManager.UpdateStatoSupportoRemoto(this._idConservazione, 0, "I");

                // 1. Reperimento path file chiusura
                XmlDocument xmlFile = new XmlDocument();

                Stream fileChiusura;
                fileChiusura = new MemoryStream(new FileManager().getFileFromStore(this._idConservazione, "\\Chiusura\\file_chiusura.xml", false));
                fileChiusura.Seek(0, SeekOrigin.Begin);

                xmlFile.Load(fileChiusura);

                XmlNodeList nodes = xmlFile.GetElementsByTagName("sincro:File");
                this._statoVerifica.TotaleDocumenti = nodes.Count;

                foreach (XmlNode n in nodes)
                {
                    if (this._interrompi)
                        throw new ApplicationException(string.Format("Verifica integrità interrotta. Verificati {0} documenti su {1}.", this._statoVerifica.DocumentiVerificati, this._statoVerifica.TotaleDocumenti));

                    // 2. Estrazione dei documenti conservati da file di chiusura

                    // Reperimento dei metadati dei file conservati dal file di chiusura
                    string idDocumento = ((XmlNode)n.ChildNodes[0]).InnerText;
                    string pathFile = ((XmlNode)n.ChildNodes[1]).InnerText;
                    string hashSupporto = ((XmlNode)n.ChildNodes[2]).InnerText;

                    // Caricamento hash da repository
                    string hashDbRepository, hashFileRepository;
                    this.LoadHashDocumento(idDocumento, out hashDbRepository, out hashFileRepository);

                    byte[] content = new FileManager().getFileFromStore(this._idConservazione, pathFile, false);
                    if (content != null)
                    {
                        string hashFileSupporto = this.GetHash256(content);

                        if (hashFileSupporto != hashSupporto)
                        {
                            hashFileSupporto = this.GetHash128(content);

                            if (hashFileSupporto != hashSupporto)
                                this._statoVerifica.DocumentiNonValidi++;
                        }
                    }
                    else
                    {
                        this._statoVerifica.DocumentiNonValidi++;
                    }

                    this._statoVerifica.DocumentiVerificati++;

                    // Calcolo della percentuale per lo stato di avanzamento
                    this._statoVerifica.Percentuale = (this._statoVerifica.DocumentiVerificati * 100 / nodes.Count);
                    this._statoVerifica.Descrizione = string.Format("Verifica integrità documento {0} di {1} in corso...", this._statoVerifica.DocumentiVerificati, nodes.Count);

                    // Impostazione del supporto in stato "InVerifica"
                    DocsPaConsManager.UpdateStatoSupportoRemoto(this._idConservazione, this._statoVerifica.Percentuale, "I");
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                errorMessage = ex.Message;

                logger.ErrorFormat("Errore in VerificaSupportoRemoto.IniziaVerificaSupporto: {0}", ex.ToString());
            }
            finally
            {
                if (hasError)
                {
                    this._statoVerifica.Esito = false;
                    this._statoVerifica.Percentuale = 0;
                    this._statoVerifica.Descrizione = errorMessage;
                }
                else
                {
                    this._statoVerifica.Esito = (this._statoVerifica.DocumentiVerificati == this._statoVerifica.TotaleDocumenti);

                    if (this._statoVerifica.Esito)
                    {
                        this._statoVerifica.Percentuale = 100;
                        this._statoVerifica.Descrizione = string.Format("Verifica integrità terminata. Verificati {0} su {1} documenti. Supporto valido.", this._statoVerifica.DocumentiVerificati, this._statoVerifica.TotaleDocumenti);
                    }
                    else
                    {
                        this._statoVerifica.Percentuale = 0;
                        this._statoVerifica.Descrizione = string.Format("Verifica integrità terminata. Individuati {0} documenti non validi su {1}. Supporto corrotto.", this._statoVerifica.DocumentiNonValidi, this._statoVerifica.TotaleDocumenti);
                    }
                }
                BusinessLogic.UserLog.UserLog.WriteLog(this._infoUtente,
                           "INTEGRITA_STORAGE",
                           this._idConservazione,
                           String.Format("Esecuzione della verifica di integrità dei documenti dell’istanza {0}", this._idConservazione),
                           (this._statoVerifica.Esito ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO));
                DocsPaConsManager consManager = new DocsPaConsManager();
                // Modifica per inserimento in dPA_CONS_VERIFICA
                consManager.RegistraEsitoVerificaSupportoRegistrato(
                                           this._infoUtente,
                                           this._idConservazione,
                                           FetchIdSupporto(),
                                           this._statoVerifica.Esito,
                                           "100",
                                           FetchDataProssimaVerifica(),
                                           "Verifica Integrità per chiusura",
                                           "C");


                // Modifica scrittura Log e Registro di Conservazione per la scrittura della Creazione Storage
                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = this._infoUtente.idAmministrazione;
                regCons.idIstanza = this._idConservazione;
                regCons.tipoOggetto = "I";
                regCons.tipoAzione = "";
                regCons.userId = this._infoUtente.userId;
                regCons.codAzione = "INTEGRITA_STORAGE";
                regCons.descAzione = "Esecuzione della verifica di integrità dei documenti istanza " + this._idConservazione;
                regCons.esito = (this._statoVerifica.Esito ? "1" : "0");
                RegistroConservazione rc = new RegistroConservazione();
                rc.inserimentoInRegistroCons(regCons, this._infoUtente);
                // Aggiornamento dello stato del supporto
                this.UpdateIstanzaConservazione();
            }

            logger.Debug("END - IniziaVerificaSupporto");
        }

        /// <summary>
        /// Aggiornamento dello stato dell'istanza di conservazione e del supporto
        /// </summary>
        protected virtual void UpdateIstanzaConservazione()
        {
            string descrizione, codAzione;
            logger.Info("UpdateIstanzaConservazione");

            if (this._statoVerifica.Esito)
            {
                string stato = string.Empty;

                int countSupportiRimovibiliRegistrati = DocsPaConsManager.GetCountSupportiRimovibiliRegistrati(this._idConservazione);
                int countSupportiRimovibili = DocsPaConsManager.GetCountSupportiRimovibili(this._idConservazione);

                logger.InfoFormat("UpdateIstanzaConservazione.countSupportiRimovibiliRegistrati: {0}", countSupportiRimovibiliRegistrati);
                logger.InfoFormat("UpdateIstanzaConservazione.countSupportiRimovibili: {0}", countSupportiRimovibili);

                // Modifica per scrittura Log e Registro di conservazione
                if (countSupportiRimovibiliRegistrati == countSupportiRimovibili)
                {
                    stato = DocsPaConservazione.StatoIstanza.CHIUSA;
                    descrizione = "Passaggio in stato CHIUSA dell’istanza " + _idConservazione;
                    codAzione = "PASS_ISTANZA_CHIUSA";
                }
                else
                {
                    stato = DocsPaConservazione.StatoIstanza.CONSERVATA;
                    descrizione = "Passaggio in stato CONSERVATA dell’istanza " + _idConservazione;
                    codAzione = "PASS_ISTANZA_CONSERVATA";
                }

                // Impostazione dello stato dell'istanza allo stato "CONSERVATO"
                DocsPaConsManager.UpdateStatoIstanzaConservazione(this._idConservazione, stato);

                //Scrive il log
                BusinessLogic.UserLog.UserLog.WriteLog(_infoUtente,
                                       codAzione,
                                       _idConservazione,
                                       descrizione,
                                       DocsPaVO.Logger.CodAzione.Esito.OK);

                // Scrive sul Registro di Conservazione
                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = _infoUtente.idAmministrazione;
                regCons.idIstanza = _idConservazione;
                regCons.tipoOggetto = "I";
                regCons.tipoAzione = "";
                regCons.userId = _infoUtente.userId;
                regCons.codAzione = codAzione;
                regCons.descAzione = descrizione;
                regCons.esito = "1";
                RegistroConservazione rc = new RegistroConservazione();
                rc.inserimentoInRegistroCons(regCons, _infoUtente);

            }
            else
            {
                logger.Debug("Si è verificato un errore durante lo spostamento dei file sullo storage remoto. L'istanza torna nello stato Firmata");
                DocsPaConsManager.UpdateStatoIstanzaConservazione(_idConservazione, DocsPaConservazione.StatoIstanza.FIRMATA);
            }

            DocsPaConsManager.UpdateStatoSupportoRemoto(this._idConservazione, 100, (this._statoVerifica.Esito ? StatoSupporto.VERIFICATO : StatoSupporto.DANNEGGIATO));

            logger.Info("UpdateIstanzaConservazione - END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string GetHash256(byte[] content)
        {
            return DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string GetHash128(byte[] content)
        {
            return DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="hashDb"></param>
        /// <param name="hashFile"></param>
        protected void LoadHashDocumento(string idDocumento, out string hashDb, out string hashFile)
        {
            hashDb = string.Empty;
            hashFile = string.Empty;

            DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(this._infoUtente, idDocumento);

            DocsPaVO.documento.FileRequest fr = sd.documenti[0] as DocsPaVO.documento.FileRequest;

            if (fr != null)
            {
                DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, this._infoUtente, false);

                using (DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti())
                    documentiDb.GetImpronta(out hashDb, fr.versionId, idDocumento);

                hashFile = this.GetHash256(fd.content);

                if (hashFile != hashDb)
                    hashFile = this.GetHash128(fd.content);
            }
        }


    }
}
