using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DocsPaWS
{
    /// <summary>
    /// DocsPa 3.0 DocsPaWSLite
    /// </summary>
    /// <remarks>
    /// Web Service semplificato
    /// </remarks>

    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [WebService(Namespace = "http://www.etnoteam.it/docspawslite/")]
    public class DocsPaWSLite : System.Web.Services.WebService
    {

        private string UserID;
        private string Password;
        private DocsPaVO.utente.Utente Utente;
        private DocsPaVO.utente.Ruolo Ruolo;
        private DocsPaWS.DocsPaWebService WS;
        private string IdAddress;
        private string SessionIdWSLite = "WSLite";
        protected DocsPaVO.utente.UserLogin userLogin;


        public DocsPaWSLite()
        {
            InitializeComponent();
        }

        #region Component Designer generated code

        private IContainer components = null;


        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion


        /// <summary>
        /// Web method di protocollazione semplificata 
        /// </summary>
        /// <param name="Oggetto"></param>
        /// <param name="Mittente"></param>
        /// <param name="numProto"></param>
        /// <param name="AnnoProto"></param>
        /// <returns>
        ///  numero e anno di protocollazione come parametri di output
        ///  true se esecuzione ok, false se errore 
        /// </returns>
        /// <remarks>
        ///  Errori gestiti: parametri oggetto e mittente obbligatori, amministrazione, utente e
        ///  ruolo non validi, stato registro diverso da 'A' e data apertura diverso da data odierna
        /// </remarks> 

        [WebMethod]
        public virtual bool Protocollazione(string User_ID, string Pwd, string Oggetto, string Mittente,
            out int numProto, out int AnnoProto, out string segnatura, out string dataProtocollo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "Protocollazione");
            
            numProto = 0;
            AnnoProto = 0;
            #region verifica dati input
            if (User_ID.Equals(""))
            {
                throw (new Exception("Campo UserID obbligatorio"));
            }

            if (Pwd.Equals(""))
            {
                throw (new Exception("Campo password obbligatorio"));
            }


            if (Oggetto.Equals(""))
            {
                throw (new Exception("Campo oggetto obbligatorio"));
            }

            if (Mittente.Equals(""))
            {
                throw (new Exception("Campo mittente obbligatorio"));
            }
            #endregion verifica dati input

            try
            {
                this.canaleWSAperto();

                this.UserID = User_ID;
                this.Password = Pwd;

                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();

                DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();

                DocsPaVO.utente.Corrispondente corr = null;
                if ((Mittente != null
                    && Mittente != ""))
                {
                    corr = new DocsPaVO.utente.Corrispondente();
                    corr.descrizione = Mittente;
                    corr.tipoCorrispondente = "O";
                }
                schedaDoc = this.creaSchedaDocumento(Oggetto, "", corr, schedaDoc);

                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione;
                DocsPaVO.documento.SchedaDocumento schedaResult = this.WS.DocumentoProtocolla(schedaDoc, infoUtente, this.Ruolo, out risultatoProtocollazione);
                // controllo i risultati 
                if (schedaResult != null && risultatoProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK))
                {
                    #region riempi parametri di output

                    numProto = Int32.Parse(schedaResult.protocollo.numero);
                    AnnoProto = Int32.Parse(schedaResult.protocollo.anno);
                    segnatura = schedaResult.protocollo.segnatura;
                    dataProtocollo = schedaResult.protocollo.dataProtocollazione;
                    dataProtocollo = DateTime.Parse(dataProtocollo, new System.Globalization.CultureInfo("it-IT", false), System.Globalization.DateTimeStyles.None).ToString();
                    dataProtocollo = dataProtocollo.Replace("-", "/");
                    dataProtocollo = dataProtocollo.Substring(0, 10);

                    #endregion riempi parametri di output
                }
                else //	risultatoProtocollazione != DocsPaWR.ResultProtocollazione.OK)
                {
                    #region gestione errore
                    System.Text.StringBuilder descErr = new System.Text.StringBuilder("Errore in fase di protocollazione:", 100);
                    switch (risultatoProtocollazione)
                    {
                        case DocsPaVO.documento.ResultProtocollazione.REGISTRO_CHIUSO:
                            descErr.Append(" Registro chiuso.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.STATO_REGISTRO_ERRATO:
                            descErr.Append(" Stato del Registro errato.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_ERRATA:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                            descErr.Append(" Formato della segnatura non impostato. Contattare l'Amministratore");
                            break;
                        default:
                            descErr.AppendFormat(" Nessun dettaglio sul codice errore {0}.", risultatoProtocollazione.ToString());
                            break;
                    }
                    throw (new Exception(descErr.ToString()));

                    #endregion gestione errore
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
            return true;
        }

        /// <summary>
        /// Web Method semplificato per l'inserimento di un corrispondente esterno
        /// </summary>
        /// <returns></returns>

        [WebMethod]
        public virtual bool InsertCorrispondenteEsterno(string userid, string pwd, string codice, string codrubrica,
            string descrizione, string tipo_urp, string email, string indirizzo, string cap, string citta, string prov,
            string cognome, string nome)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "InsertCorrispondenteEsterno");

            #region Controllo dati in input

            if (userid == null || userid.Length == 0)
                throw (new Exception("Campo UserID mancante"));

            if (pwd == null || pwd.Length == 0)
                throw (new Exception("Campo password mancante"));

            if (codice == null || codice.Length == 0)
                throw (new Exception("Campo codice mancante"));

            if (codrubrica == null || codrubrica.Length == 0)
                throw (new Exception("Campo codice rubrica mancante"));

            if (tipo_urp == null || tipo_urp.Length == 0 || (tipo_urp.ToUpper() != "U" && tipo_urp.ToUpper() != "P"))
                throw (new Exception("Tipo corrispondente (U/P) errato o mancante"));

            if (tipo_urp == "P")
            {
                if (cognome == null || cognome.Length == 0)
                    throw (new Exception("Campo cognome mancante"));
                if (nome == null || nome.Length == 0)
                    throw (new Exception("Campo nome mancante"));
            }

            if (tipo_urp == "U")
            {
                if (descrizione == null || descrizione.Length == 0)
                    throw (new Exception("Campo descrizione mancante"));
            }

            if (email != null && email.Length > 0)
            {
                if (!ValidateEmail(email))
                    throw (new Exception("Indirizzo email non conforme"));
            }

            #endregion

            try
            {
                this.canaleWSAperto();

                this.UserID = userid;
                this.Password = pwd;

                if (!Login(this.UserID, this.Password))
                    throw (new Exception("Errore durante il login dell'utente"));

                return insertCorrEsterno(codice, codrubrica, descrizione, tipo_urp.ToUpper(), email, indirizzo, cap, citta, prov,
                    cognome, nome);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }

        }

        [WebMethod]
        [System.Xml.Serialization.XmlInclude(typeof(DocsPaVO.utente.UnitaOrganizzativa))]
        public DocsPaVO.documento.SchedaDocumento ricercaSchedabyChiaveProto(string numproto, string anno, string tipodoc, string codreg, string userId, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "ricercaSchedabyChiaveProto");

            DocsPaVO.documento.SchedaDocumento sch = null;
            try
            {
                this.canaleWSAperto();

                this.UserID = userId;
                this.Password = password;

                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }
                else
                {

                    DocsPaVO.utente.InfoUtente infout = this.getInfoUtenteAttuale();
                    sch = BusinessLogic.Documenti.ProtoManager.ricercaSchedaByTipoDoc(numproto, anno, codreg, tipodoc, infout);
                }
                return sch;
            }
            catch (Exception e)
            { throw e; }

            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }

        }



        #region protocollazioneEstesa e protocollazioneEstesaConClassifica
        /// <summary>
        /// 
        /// </summary>
        /// <param name="User_ID"></param>
        /// <param name="Pwd"></param>
        /// <param name="Oggetto"></param>
        /// <param name="note"></param>
        /// <param name="TipoProtocollo"></param>
        /// <param name="corrispondenti"></param>
        /// <param name="tipologiaDocumento"></param>
        /// <param name="modelloTrasmissione"></param>
        /// <param name="numProto"></param>
        /// <param name="AnnoProto"></param>
        /// <param name="segnatura"></param>
        /// <param name="dataProtocollo"></param>
        /// <returns></returns>
        [WebMethod]
        public virtual DocsPaVO.ProfilazioneDinamicaLite.Result protocollazioneDocumentoProfilato(string User_ID, string Pwd, string Oggetto, string note,
            string TipoProtocollo, CorrLite[] corrispondenti, DocsPaVO.ProfilazioneDinamicaLite.TipologiaDocProfilato tipologiaDocumento,
            string modelloTrasmissione, out int numProto, out int AnnoProto, out string segnatura, out string dataProtocollo)
        {

            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "protocollazioneDocumentoProfilato");
            bool result = false;
            numProto = 0;
            AnnoProto = 0;
            segnatura = "";
            dataProtocollo = "";
            this.canaleWSAperto();
            DocsPaVO.ProfilazioneDinamicaLite.Result risultato = new DocsPaVO.ProfilazioneDinamicaLite.Result();
            risultato.codice = -1;
            risultato.descrizione = "Operazione non effettuata.";

            try
            {

                this.UserID = User_ID;
                this.Password = Pwd;

                if (!Login(this.UserID, this.Password))
                {
                    risultato.codice = -1;
                    risultato.descrizione = "Operazione non effettuata (nome utente o password errate)";
                    return risultato;
                }
                
                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();

                if (this.Ruolo.idRegistro == null || this.Ruolo.idRegistro.Equals(""))
                {
                    if (Ruolo.registri != null && Ruolo.registri.Count > 0)
                        Ruolo.idRegistro = ((DocsPaVO.utente.Registro)Ruolo.registri[0]).systemId;
                }

                //gestione tipologia documento
                if (tipologiaDocumento == null)
                {
                    risultato.codice = -1;
                    risultato.descrizione = "Operazione non effettuata perchè non è stata specificata la tipologia di documento";
                    return risultato;
                }
                // inizio profilazione dinamica
                ArrayList listaTipologie = BusinessLogic.Documenti.DocManager.getTipoAttoPDInsRic(infoUtente.idAmministrazione, infoUtente.idGruppo, "2");//BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplates(infoUtente.idAmministrazione);
                DocsPaVO.documento.TipologiaAtto tipoAtto = new DocsPaVO.documento.TipologiaAtto();
                bool trovato = false;

                if (listaTipologie.Count > 0)
                {
                    for (int i = 0; i < listaTipologie.Count && !trovato; i++)
                    {
                        tipoAtto = (DocsPaVO.documento.TipologiaAtto)listaTipologie[i];
                        if (tipoAtto.descrizione.ToUpper() == tipologiaDocumento.nome.ToUpper())
                        {
                            trovato = true;
                            break;
                        }
                    }
                }
                else// domandare a sabrina :):)
                {
                    risultato.codice = -1;
                    risultato.descrizione = "Operazione non effettuata. Il nome della tipologia di documento non esiste o non è associata all'utente.";
                    return risultato;
                }
                if (!trovato)
                {
                    risultato.codice = -1;
                    risultato.descrizione = "Operazione non effettuata. Il nome della tipologia di documento non esiste o non è associata all'utente.";
                    return risultato;
                }
                DocsPaVO.ProfilazioneDinamica.Templates template;
                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(tipoAtto.systemId.ToString());

                //valorizzazione del templates
                for (int j = 0; j < tipologiaDocumento.listaCampiProfilati.Length; j++)
                {
                    bool trovatoCampo = false;
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                    {
                        if (ogg.DESCRIZIONE.ToUpper() == tipologiaDocumento.listaCampiProfilati[j].nomeCampo)
                        {
                            trovatoCampo = true;
                            switch (ogg.TIPO.DESCRIZIONE_TIPO.ToUpper())
                            {
                                case "CORRISPONDENTE":
                                    for (int k = 0; k < corrispondenti.Length; k++)
                                    {
                                        if (!string.IsNullOrEmpty(corrispondenti[k].codice)
                                             && corrispondenti[k].codice.ToUpper().Equals(tipologiaDocumento.listaCampiProfilati[j].valoreCampo.ToUpper()))
                                        {
                                            DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(corrispondenti[k].codice, infoUtente);
                                            if (corr != null)
                                                ogg.VALORE_DATABASE = corr.systemId;
                                            break;
                                        }
                                    }
                                    break;
                                case "CASELLADISELEZIONE":
                                    ArrayList lista = new ArrayList() { tipologiaDocumento.listaCampiProfilati[j].valoreCampo };
                                    ogg.VALORI_SELEZIONATI = lista;
                                    break;
                                default:
                                    ogg.VALORE_DATABASE = tipologiaDocumento.listaCampiProfilati[j].valoreCampo;
                                    break;
                            }
                            break;
                        }
                    }
                    if (!trovatoCampo)
                    {
                        risultato.codice = -1;
                        risultato.descrizione = "Operazione non effettuata. Il campo " + tipologiaDocumento.listaCampiProfilati[j] .nomeCampo + " non è presente nella tipologia specificata.";
                        return risultato;
                    }
                }
                

                DocsPaVO.documento.SchedaDocumento schedaResult = new DocsPaVO.documento.SchedaDocumento();
                schedaResult.tipologiaAtto = tipoAtto;

                schedaResult.template = template;
                
                result = this.protocollazioneRef(User_ID, Pwd, Oggetto, note, TipoProtocollo,
                                              corrispondenti, ref schedaResult);

                // controllo i risultati 
                if (schedaResult != null && schedaResult.systemId != null)
                {
                    #region riempi parametri di output

                    numProto = Int32.Parse(schedaResult.protocollo.numero);
                    AnnoProto = Int32.Parse(schedaResult.protocollo.anno);
                    segnatura = schedaResult.protocollo.segnatura;
                    dataProtocollo = schedaResult.protocollo.dataProtocollazione;
                    dataProtocollo = DateTime.Parse(dataProtocollo, new System.Globalization.CultureInfo("it-IT", false), System.Globalization.DateTimeStyles.None).ToString();
                    dataProtocollo = dataProtocollo.Replace("-", "/");
                    dataProtocollo = dataProtocollo.Substring(0, 10);

                    if (result)
                    {
                        risultato.codice = 1;
                        risultato.descrizione = "Protocollazione effettuata";
                    }
                    else
                    {
                        risultato.codice = -1;
                        risultato.descrizione = "Protocollazione non effettuata";
                        return risultato;
                    }
                    if (!string.IsNullOrEmpty(modelloTrasmissione))
                    {
                        result = this.TrasmissioneExecuteTrasmDocDaModello(infoUtente, null, schedaResult, modelloTrasmissione);

                        if (result)
                        {
                            risultato.codice = 0;
                            risultato.descrizione = "Operazione effettuata con successo (Protocollazione e Trasmissione)";
                        }
                    }
                    else
                    {
                        risultato.codice = 0;
                        risultato.descrizione = "Operazione effettuata con successo (Protocollazione - Trasmissione non richiesta)";
                        return risultato;
                    }

                    #endregion riempi parametri di output
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }

            
            return risultato;
        }


        [WebMethod]
        public virtual bool protocollazioneEstesa(string User_ID, string Pwd, string Oggetto, string note, string TipoProtocollo, CorrLite[] corrispondenti,
            out int numProto, out int AnnoProto, out string segnatura, out string dataProtocollo)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "protocollazioneEstesa");
            bool result = false;
            numProto = 0;
            AnnoProto = 0;
            segnatura = "";
            dataProtocollo = "";
            try
            {

                this.canaleWSAperto();
                DocsPaVO.documento.SchedaDocumento schedaResult;
                result = this.protocollazione(User_ID, Pwd, Oggetto, note, TipoProtocollo,
                                              corrispondenti, out schedaResult);

                // controllo i risultati 
                if (schedaResult != null && schedaResult.systemId != null)
                {
                    #region riempi parametri di output

                    numProto = Int32.Parse(schedaResult.protocollo.numero);
                    AnnoProto = Int32.Parse(schedaResult.protocollo.anno);
                    segnatura = schedaResult.protocollo.segnatura;
                    dataProtocollo = schedaResult.protocollo.dataProtocollazione;
                    dataProtocollo = DateTime.Parse(dataProtocollo, new System.Globalization.CultureInfo("it-IT", false), System.Globalization.DateTimeStyles.None).ToString();
                    dataProtocollo = dataProtocollo.Replace("-", "/");
                    dataProtocollo = dataProtocollo.Substring(0, 10);

                    #endregion riempi parametri di output
                    result = true;
                }

            }
            catch (Exception e)
            {
                throw e;
                result = false;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }

            return result;
        }

        [WebMethod]
        public virtual bool protocollazioneEstesaConClass(string User_ID, string Pwd, string Oggetto, string note, string TipoProtocollo, CorrLite[] corrispondenti, string codFascicolo,
            out int numProto, out int AnnoProto, out string segnatura, out string dataProtocollo, out string errMessage)
        {
            
                

            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "protocollazioneEstesaConClass");
            bool result = false;
            numProto = 0;
            AnnoProto = 0;
            segnatura = "";
            dataProtocollo = "";
            errMessage = "";
            try
            {
                this.canaleWSAperto();

                DocsPaVO.documento.SchedaDocumento schedaResult;
                result = this.protocollazione(User_ID, Pwd, Oggetto, note, TipoProtocollo,
                                              corrispondenti, out schedaResult);

                DocsPaVO.utente.InfoUtente infoUt = this.getInfoUtenteAttuale();

                // controllo i risultati della protocollazione
                if (result && schedaResult != null && schedaResult.systemId != null)
                {
                    #region riempi parametri di output

                    numProto = Int32.Parse(schedaResult.protocollo.numero);
                    AnnoProto = Int32.Parse(schedaResult.protocollo.anno);
                    segnatura = schedaResult.protocollo.segnatura;
                    dataProtocollo = schedaResult.protocollo.dataProtocollazione;
                    dataProtocollo = DateTime.Parse(dataProtocollo, new System.Globalization.CultureInfo("it-IT", false), System.Globalization.DateTimeStyles.None).ToString();
                    dataProtocollo = dataProtocollo.Replace("-", "/");
                    dataProtocollo = dataProtocollo.Substring(0, 10);

                    #endregion riempi parametri di output
                    result = true;
                }
                else
                {
                    errMessage = "Errore nella protocollazione";
                    return false;
                }
                //classificazione
                string msg = string.Empty;
                if (codFascicolo == null || codFascicolo.Trim().Equals(""))
                {
                    errMessage = "Fascicolo non specificato";
                    return result;
                }


                DocsPaVO.fascicolazione.Fascicolo fascicolo = this.WS.FascicolazioneGetFascicoloDaCodice(infoUt, codFascicolo, schedaResult.registro, false, false);

                if (fascicolo != null && fascicolo.systemID != null)
                {
                    bool res = this.WS.FascicolazioneAddDocFascicolo(infoUt, schedaResult.systemId, fascicolo, false, out msg);
                    if (!res)
                    {
                        errMessage = "Si è verificato un errore nella classificazione del documento";
                    }
                }
                else
                {
                    errMessage = "Fascicolo non trovato";
                }
            }
            catch (Exception e)
            {
                throw e;
                result = false;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
            return result;
        }

        private void putinfoCorr(DocsPaVO.documento.SchedaDocumento sch)
        {
            
            if (sch != null && sch.protocollo != null)
            {
                if (sch.tipoProto.Equals("A"))
                {
                    DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)sch.protocollo;
                    if (pe.mittente != null
                        && pe.mittente.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pe.mittente);
                    if (pe.mittenteIntermedio != null
                        && pe.mittenteIntermedio.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pe.mittenteIntermedio);

                }
                if (sch.tipoProto.Equals("P"))
                {
                    DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)sch.protocollo;
                    if (pu.mittente != null
                        && pu.mittente.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pu.mittente);
                    if (pu.destinatari != null
                        && pu.destinatari.Count > 0)
                        for (int i = 0; i < pu.destinatari.Count; i++)
                        {
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)pu.destinatari[i];
                            if (corr != null && corr.info != null)
                                BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(corr);
                        }

                }
                if (sch.tipoProto.Equals("I"))
                {
                    DocsPaVO.documento.ProtocolloInterno pi = (DocsPaVO.documento.ProtocolloInterno)sch.protocollo;
                    if (pi.mittente != null
                        && pi.mittente.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pi.mittente);
                    if (pi.destinatari != null
                        && pi.destinatari.Count > 0)
                        for (int i = 0; i < pi.destinatari.Count; i++)
                        {
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)pi.destinatari[i];
                            if (corr != null && corr.info != null)
                                BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(corr);
                        }

                }

            }
        }
        #region struttura di supporto

        public struct CorrLite
        {
            public string descrizione;
            public string codice;
            public string tipoCorrispondente;
            public string indirizzo;
            public string cap;
            public string citta;
            public string provincia;
            public string nazione;
            public string telefono;
            public string telefono2;
            public string fax;
            public string codiceFiscale;
            public string note;
            public string localita;
        }




        #endregion

        protected bool protocollazione(string User_ID, string Pwd, string Oggetto, string note, string TipoProtocollo, CorrLite[] corrispondenti,
            out DocsPaVO.documento.SchedaDocumento schedaResult)
        {

            bool result;

            #region verifica dati input
            if (User_ID.Equals(""))
            {
                throw (new Exception("Campo UserID obbligatorio"));
            }

            if (Pwd.Equals(""))
            {
                throw (new Exception("Campo password obbligatorio"));
            }
            if (Oggetto.Equals(""))
            {
                throw (new Exception("Campo oggetto obbligatorio"));
            }
            if (TipoProtocollo.Equals(""))
            {
                throw (new Exception("Campo TipoProtocollo obbligatorio"));
            }
            if (corrispondenti.Length == 0)
            {
                throw (new Exception("Campo corrispondenti obbligatorio"));
            }
            #endregion verifica dati input

            try
            {
                this.UserID = User_ID;
                this.Password = Pwd;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();
                DocsPaVO.utente.Corrispondente corr = null;
                DocsPaVO.documento.ProtocolloUscita protoOUT = null;
                DocsPaVO.documento.ProtocolloEntrata protoIN = null;

                DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();

                #region Tipologia Protocollo
                // protocollo in partenza
                if (TipoProtocollo == "P")
                {
                    protoOUT = new DocsPaVO.documento.ProtocolloUscita();

                    for (int i = 0; i < corrispondenti.Length; i++)
                    {
                        // verifica preesistenza della descrizione del corrispondente
                        //						if(corrispondenti[i].descrizione.Trim().Equals("") 
                        //							throw (new Exception("Campo UserID obbligatorio"));
                        corr = this.ricercaCorr(corrispondenti[i], infoUtente);
                        corr = setInfoCorr(corr, corrispondenti[i]);

                        // un solo mittente opzionale	
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "M")
                        {
                            protoOUT.mittente = corr;
                        }
                        // uno o più destinatari
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "D")
                        {
                            if (protoOUT.destinatari == null)
                                protoOUT.destinatari = new ArrayList();
                            protoOUT.destinatari.Add(corr);
                        }
                        // uno o più destinatari per conoscenza
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "C")
                        {
                            if (protoOUT.destinatariConoscenza == null)
                                protoOUT.destinatariConoscenza = new ArrayList();
                            protoOUT.destinatariConoscenza.Add(corr);
                        }

                    }
                    schedaDoc.protocollo = protoOUT;
                    schedaDoc.tipoProto = "P";
                }

                // protocollo in arrivo
                if (TipoProtocollo == "A")
                {
                    protoIN = new DocsPaVO.documento.ProtocolloEntrata();

                    for (int i = 0; i < corrispondenti.Length; i++)
                    {

                        corr = this.ricercaCorr(corrispondenti[i], infoUtente);
                        corr = setInfoCorr(corr, corrispondenti[i]);

                        //un solo mittente
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "M")
                        {

                            protoIN.mittente = corr;
                        }
                        //un solo mittente Intermedio
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "I")
                        {
                            protoIN.mittenteIntermedio = corr;
                        }
                    }
                    schedaDoc.protocollo = protoIN;
                    schedaDoc.tipoProto = "A";
                }

                #endregion

                #region Protocollazione

                schedaDoc = this.creaSchedaDocumento(Oggetto, note, corr, schedaDoc);


                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione;
                schedaResult = this.WS.DocumentoProtocolla(schedaDoc, infoUtente, this.Ruolo, out risultatoProtocollazione);

                //inserisci i dettagli dei corrispondneti del protocollo
                if (schedaResult != null)
                    this.putinfoCorr(schedaResult);
                // controllo i risultati 
                if (schedaResult == null || !risultatoProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK)) //	risultatoProtocollazione != DocsPaWR.ResultProtocollazione.OK)
                {
                    #region gestione errore
                    System.Text.StringBuilder descErr = new System.Text.StringBuilder("Errore in fase di protocollazione:", 100);
                    switch (risultatoProtocollazione)
                    {
                        case DocsPaVO.documento.ResultProtocollazione.REGISTRO_CHIUSO:
                            descErr.Append(" Registro chiuso.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.STATO_REGISTRO_ERRATO:
                            descErr.Append(" Stato del Registro errato.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_ERRATA:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                            descErr.Append(" Formato della segnatura non impostato. Contattare l'Amministratore");
                            break;
                        default:
                            descErr.AppendFormat(" Nessun dettaglio sul codice errore {0}.", risultatoProtocollazione.ToString());
                            break;
                    }
                    throw (new Exception(descErr.ToString()));

                    #endregion gestione errore
                }
                #endregion

                result = true;
            }
            catch (Exception e)
            {
                throw e;
                result = false;
            }
            //finally
            //{
            //    this.Logoff();
            //    this.chiudiCanaleWS();

            //}
            return result;
        }

        #endregion


        protected bool protocollazioneRef(string User_ID, string Pwd, string Oggetto, string note, string TipoProtocollo, CorrLite[] corrispondenti,
            ref DocsPaVO.documento.SchedaDocumento schedaResult)
        {
            bool result;

            #region verifica dati input
            if (User_ID.Equals(""))
            {
                throw (new Exception("Campo UserID obbligatorio"));
            }

            if (Pwd.Equals(""))
            {
                throw (new Exception("Campo password obbligatorio"));
            }
            if (Oggetto.Equals(""))
            {
                throw (new Exception("Campo oggetto obbligatorio"));
            }
            if (TipoProtocollo.Equals(""))
            {
                throw (new Exception("Campo TipoProtocollo obbligatorio"));
            }
            if (corrispondenti.Length == 0)
            {
                throw (new Exception("Campo corrispondenti obbligatorio"));
            }
            #endregion verifica dati input

            try
            {
                this.UserID = User_ID;
                this.Password = Pwd;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();
                DocsPaVO.utente.Corrispondente corr = null;
                DocsPaVO.documento.ProtocolloUscita protoOUT = null;
                DocsPaVO.documento.ProtocolloEntrata protoIN = null;

                DocsPaVO.documento.SchedaDocumento schedaDoc;
                if (schedaResult != null)
                    schedaDoc = schedaResult;
                else
                    schedaDoc = new DocsPaVO.documento.SchedaDocumento();

                #region Tipologia Protocollo
                // protocollo in partenza
                if (TipoProtocollo == "P")
                {
                    protoOUT = new DocsPaVO.documento.ProtocolloUscita();

                    for (int i = 0; i < corrispondenti.Length; i++)
                    {
                        // verifica preesistenza della descrizione del corrispondente
                        //						if(corrispondenti[i].descrizione.Trim().Equals("") 
                        //							throw (new Exception("Campo UserID obbligatorio"));
                        corr = this.ricercaCorr(corrispondenti[i], infoUtente);
                        corr = setInfoCorr(corr, corrispondenti[i]);

                        // un solo mittente opzionale	
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "M")
                        {
                            protoOUT.mittente = corr;
                        }
                        // uno o più destinatari
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "D")
                        {
                            if (protoOUT.destinatari == null)
                                protoOUT.destinatari = new ArrayList();
                            protoOUT.destinatari.Add(corr);
                        }
                        // uno o più destinatari per conoscenza
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "C")
                        {
                            if (protoOUT.destinatariConoscenza == null)
                                protoOUT.destinatariConoscenza = new ArrayList();
                            protoOUT.destinatariConoscenza.Add(corr);
                        }

                    }
                    schedaDoc.protocollo = protoOUT;
                    schedaDoc.tipoProto = "P";
                }

                // protocollo in arrivo
                if (TipoProtocollo == "A")
                {
                    protoIN = new DocsPaVO.documento.ProtocolloEntrata();

                    for (int i = 0; i < corrispondenti.Length; i++)
                    {

                        corr = this.ricercaCorr(corrispondenti[i], infoUtente);
                        corr = setInfoCorr(corr, corrispondenti[i]);

                        //un solo mittente
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "M")
                        {

                            protoIN.mittente = corr;
                        }
                        //un solo mittente Intermedio
                        if (((CorrLite)corrispondenti[i]).tipoCorrispondente == "I")
                        {
                            protoIN.mittenteIntermedio = corr;
                        }
                    }
                    schedaDoc.protocollo = protoIN;
                    schedaDoc.tipoProto = "A";
                }

                #endregion

                #region Protocollazione

                schedaDoc = this.creaSchedaDocumento(Oggetto, note, corr, schedaDoc);


                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione;
                schedaResult = this.WS.DocumentoProtocolla(schedaDoc, infoUtente, this.Ruolo, out risultatoProtocollazione);

                //inserisci i dettagli dei corrispondneti del protocollo
                if (schedaResult != null)
                    this.putinfoCorr(schedaResult);
                // controllo i risultati 
                if (schedaResult == null || !risultatoProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK)) //	risultatoProtocollazione != DocsPaWR.ResultProtocollazione.OK)
                {
                    #region gestione errore
                    System.Text.StringBuilder descErr = new System.Text.StringBuilder("Errore in fase di protocollazione:", 100);
                    switch (risultatoProtocollazione)
                    {
                        case DocsPaVO.documento.ResultProtocollazione.REGISTRO_CHIUSO:
                            descErr.Append(" Registro chiuso.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.STATO_REGISTRO_ERRATO:
                            descErr.Append(" Stato del Registro errato.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_ERRATA:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                            descErr.Append(" Formato della segnatura non impostato. Contattare l'Amministratore");
                            break;
                        default:
                            descErr.AppendFormat(" Nessun dettaglio sul codice errore {0}.", risultatoProtocollazione.ToString());
                            break;
                    }
                    throw (new Exception(descErr.ToString()));

                    #endregion gestione errore
                }
                #endregion

                result = true;
            }
            catch (Exception e)
            {
                throw e;
                result = false;
            }
            //finally
            //{
            //    this.Logoff();
            //    this.chiudiCanaleWS();

            //}
            return result;
        }

    
        #region ricerca/verifica Corr
        private DocsPaVO.utente.Corrispondente setInfoCorr(DocsPaVO.utente.Corrispondente corr, CorrLite corrWs)
        {
          

            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
            DocsPaUtils.Data.TypedDataSetManager.MakeTyped(corr.info, dettagliCorrispondente.Corrispondente.DataSet);

            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();

            string indirizzo = corrWs.indirizzo != null ? corrWs.indirizzo.Trim() : "";
            string citta = corrWs.citta != null ? corrWs.citta.Trim() : "";
            string cap = corrWs.cap != null ? corrWs.cap.Trim() : "";
            string provincia = corrWs.provincia != null ? corrWs.provincia.Trim() : "";
            string nazione = corrWs.nazione != null ? corrWs.nazione.Trim() : "";
            string telefono = corrWs.telefono != null ? corrWs.telefono.Trim() : "";
            string telefono2 = corrWs.telefono2 != null ? corrWs.telefono2.Trim() : "";
            string fax = corrWs.fax != null ? corrWs.fax.Trim() : "";
            string codiceFiscale = corrWs.codiceFiscale != null ? corrWs.codiceFiscale.Trim() : "";
            string note = corrWs.note != null ? corrWs.note.Trim() : "";
            string localita = corrWs.localita != null ? corrWs.localita.Trim() : "";
            

            dettagliCorr.Corrispondente.AddCorrispondenteRow(
                indirizzo,
                citta,
                cap,
                provincia,
                nazione,
                telefono,
                telefono2,
                fax,
                codiceFiscale,
                note,
                localita,
                string.Empty,
                string.Empty,
                string.Empty,
                //string.Empty,
                string.Empty
            );

            if (indirizzo.Equals("") && citta.Equals("") && cap.Equals("")
                && provincia.Equals("") && nazione.Equals("") && telefono.Equals("")
                && telefono2.Equals("") && fax.Equals("") && codiceFiscale.Equals("") && note.Equals(""))
                corr.info = null;
            else
                corr.info = dettagliCorr;
            return corr;

        }

        private DocsPaVO.utente.Corrispondente ricercaCorr(CorrLite corrispondente, DocsPaVO.utente.InfoUtente
            infoUtente)
        {
          

            DocsPaVO.utente.Corrispondente corr = null;
            // verifica preesistenza della descrizione del corrispondente
            if (corrispondente.codice != null && corrispondente.codice != "")
            {
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.codiceRubrica = corrispondente.codice;
                qco.getChildren = false;
                qco.idAmministrazione = infoUtente.idAmministrazione;
                // secondo me è solo esterno, ma per ora lascio GLOBALE.
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;
                ArrayList corrls = this.WS.AddressbookGetListaCorrispondenti(qco);
                if (corrls != null && corrls.Count > 0)
                    corr = (DocsPaVO.utente.Corrispondente)corrls[0];
            }
            if (corr != null && corr.systemId != null && corr.systemId != "")
            {//trovato, cotrollo se la descrizione è quella inserita dall'utente:
                if (corrispondente.descrizione.ToUpper() == corr.descrizione.ToUpper())
                { //ok, è lo stesso, quindi da specifica si usa quello della rubrica di docspa
                    //uso  corr	
                }
                else
                {
                    //creo un occ con la descrizione passata dall'utente
                    corr = new DocsPaVO.utente.Corrispondente();
                    corr.descrizione = corrispondente.descrizione;
                    corr.tipoCorrispondente = "O";
                    corr.idAmministrazione = infoUtente.idAmministrazione;

                }
            }
            else
            //non trovato, CREO OCC.
            {
                corr = new DocsPaVO.utente.Corrispondente();
                corr.descrizione = corrispondente.descrizione;
                corr.tipoCorrispondente = "O";
                corr.idAmministrazione = infoUtente.idAmministrazione;

            }

            return corr;
        }

        private DocsPaVO.utente.Corrispondente ricercaCorrIE(CorrLite corrispondente, String TipoIE, DocsPaVO.utente.InfoUtente
infoUtente)
        {
            

            DocsPaVO.utente.Corrispondente corr = null;
            // verifica preesistenza della descrizione del corrispondente
            if (corrispondente.codice != null && corrispondente.codice != "")
            {
                DocsPaVO.addressbook.TipoUtente tu = DocsPaVO.addressbook.TipoUtente.INTERNO;

                if (TipoIE == "I")
                {
                    tu = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    corr = this.WS.AddressbookGetCorrispondenteByCodRubricaIE(corrispondente.codice, tu, infoUtente);
                }
                if (TipoIE == "E")
                {
                    tu = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                    corr = this.WS.AddressbookGetCorrispondenteByCodRubricaIE(corrispondente.codice, tu, infoUtente);
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    if (corr != null)
                    {
                        qco.systemId = corr.systemId;
                        corr.info = this.WS.AddressbookGetDettagliCorrispondente(qco);
                    }

                }
            }

            return corr;
        }

        #endregion

        #region login/logoff
        //		[WebMethod]
        //		public DocsPaVO.utente.InfoUtente loginLite(string userId,string password)
        //		{
        //			
        //			try
        //			{
        //				DocsPaVO.utente.InfoUtente info=null;
        //				this.canaleWSAperto();
        //				info=this.loginLite(userId,password);
        //			}
        //			catch (Exception ex)
        //			{
        //				throw ex;
        //			}
        //
        //			
        //
        //		}
        //		[WebMethod]
        //		public bool logOffLite(string userId,string password)
        //		{
        //			try
        //			{
        //				DocsPaVO.utente.InfoUtente info=null;
        //				this.canaleWSAperto();
        //				info=this.Logoff()
        //			}
        //			catch (Exception ex)
        //			{
        //				throw ex;
        //			}
        //		}
        #endregion

        #region ricercaScheda
        /// <summary>
        /// Ricerca il protocollo tramite la stringa della segnatura. Ritorna un oggetto SchedaDocumento.
        /// </summary>
        /// <param name="segnatura"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        [WebMethod]
        [System.Xml.Serialization.XmlInclude(typeof(DocsPaVO.utente.UnitaOrganizzativa))]
        public DocsPaVO.documento.SchedaDocumento ricercaSchedabySegnatura(string segnatura, string userId, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "ricercaSchedabySegnatura");

            DocsPaVO.documento.SchedaDocumento sch = null;
            try
            {
                this.canaleWSAperto();

                this.UserID = userId;
                this.Password = password;

                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infout = this.getInfoUtenteAttuale();

                sch = BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infout);
                return sch;
            }
            catch (Exception e)
            { throw e; }

            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }


        }
        //OLD WEbMethod
        //		[WebMethod]
        //		public DocsPaVO.documento.SchedaDocumento ricercaScheda(string num_proto,string annoProto,string idReg,DocsPaVO.utente.InfoUtente infoutente)
        //		{
        //			return BusinessLogic.Documenti.ProtoManager.ricercaScheda( num_proto, annoProto, idReg, infoutente);
        //				
        //		}
        private DocsPaVO.documento.SchedaDocumento ricercaScheda(string num_proto, string annoProto, string idReg, DocsPaVO.utente.InfoUtente infoutente)
        {

            return BusinessLogic.Documenti.ProtoManager.ricercaScheda(num_proto, annoProto, idReg, infoutente);

        }

        private DocsPaVO.documento.SchedaDocumento ricercaScheda(string segnatura, DocsPaVO.utente.InfoUtente infoutente)
        {

            return BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infoutente);

        }
        #endregion

        #region Upload / DownLoad file

        /// <summary>
        /// permette l'upload, della versione corrente di un protocollo.
        /// </summary>
        /// <example> Questo Esempio mostra come utilizzare il webMethod
        /// <code>
        ///   //login; --> infoutente
        ///		DocspaVO.SchedaDocumento sch=ricercaScheda(segnatura);
        ///     FileRequest fileReq =sch.documento[0]; //ultima versione;
        ///		FileDocumento fileDoc = new FileDocumento();
        ///		file=HTTPPostedFile; //filestream ottenuto da wweb o da file system.					
        ///		fileDoc.name = System.IO.Path.GetFileName(file.FileName); //nome+path del file
        ///		fileDoc.fullName = fileDoc.name;
        ///		fileDoc.contentType = file.ContentType;
        ///		fileDoc.length = file.ContentLength;
        ///		fileDoc.content = new Byte[fileDoc.length];
        ///		file.InputStream.Read(fileDoc.content, 0, fileDoc.length); //copia dello stream nell'oggetto FileDoc
        ///		fileReq.fileName=System.IO.Path.GetFileName(file.FileName);
        ///		fileReq = docsPaWS.DocumentoPutFile(fileReq, fileDoc, infoUtente);
        /// </code>
        /// </example>
        [WebMethod]
        public DocsPaVO.documento.FileRequest putfile(DocsPaVO.documento.FileRequest fileReq, DocsPaVO.documento.FileDocumento fileDoc, string userId, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "putfile");
            DocsPaVO.documento.FileRequest fileReqOut = null;
            try
            {
                this.canaleWSAperto();
                this.UserID = userId;
                this.Password = password;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();



                return fileReqOut = this.WS.DocumentoPutFile(fileReq, fileDoc, infoUtente);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
        }
        /// <summary>
        /// permette il donwload del file associato alla versione corrente del protocollo.
        /// </summary>
        /// /// <example> Questo Esempio mostra come utilizzare il webMethod
        /// <code>
        ///   //login; --> infoutente
        ///   DocsPaWR.FileDocumento fileDoc=getfile(segnatura,infoutente);
        ///		fileDoc.name //nome+path del file
        ///		fileDoc.fullName //nome file nel sistema docspa;
        ///		fileDoc.contentType ;
        ///		fileDoc.length ;
        ///		fileDoc.content ; //content per richeare uno stream da cui ottenere il file.
        /// </code>
        /// </example>
        /// <param name="segnatura"></param>
        /// <param name="infoUt"></param>
        /// <returns></returns>
        [WebMethod]
        public DocsPaVO.documento.FileDocumento getfile(string segnatura, string userId, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "getfile");
            DocsPaVO.documento.FileDocumento fileDoc = null;

            try
            {
                this.canaleWSAperto();
                this.canaleWSAperto();
                this.UserID = userId;
                this.Password = password;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUt = this.getInfoUtenteAttuale();

                DocsPaVO.documento.SchedaDocumento sch = this.ricercaScheda(segnatura, infoUt);

                if (sch != null)
                {

                    fileDoc = this.WS.DocumentoGetFile((DocsPaVO.documento.FileRequest)sch.documenti[0], infoUt);
                }
                return fileDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
        }
        #endregion

        #region Annullamento
        /// <summary>
        /// annulla protocollo
        /// </summary>
        /// <param name="User_ID"></param>
        /// <param name="Pwd"></param>
        /// <param name="numProto"></param>
        /// <param name="annoProto"></param>
        /// <param name="noteAnnullamento"></param>
        /// <returns></returns>
        [WebMethod]
        public virtual bool annullaProtocollo(string User_ID, string Pwd, string segnatura, string noteAnnullamento)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "annullaProtocollo");

            try
            {
                this.canaleWSAperto();

                this.UserID = User_ID;
                this.Password = Pwd;

                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();
                ArrayList regs = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(infoUtente.idCorrGlobali);
                string idReg = "";
                if (regs != null && regs.Count > 0)
                {
                    DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)regs[0];
                    if (reg != null)
                        idReg = reg.systemId;
                }

                DocsPaVO.documento.SchedaDocumento sch = ricercaScheda(segnatura, infoUtente);
                DocsPaVO.documento.ProtocolloAnnullato protoAnn = null;
                //TODO controlla diritti.
                if (sch != null && sch.accessRights != null && sch.accessRights != "")
                {
                    if (HMDiritti(sch.accessRights))
                    {
                        string ErrorMsg = "Attenzione, l'utente " + infoUtente.userId + " non possiede i diritti necessari per annullare il protocollo.";
                        throw new Exception(ErrorMsg);

                    }
                }

                bool annullato = false;

                if (sch != null)
                {

                    protoAnn = new DocsPaVO.documento.ProtocolloAnnullato();
                    protoAnn.autorizzazione = noteAnnullamento;
                    // la calcola il backend	protoAnn.dataAnnullamento=System.DateTime.Now.ToShortDateString().Replace(".",":");
                    annullato = this.WS.DocumentoExecAnnullaProt(infoUtente, ref sch, protoAnn);
                }

                return annullato;
            }
            catch (Exception e)
            {
                throw e;
                return false;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();

            }
        }

        #endregion

        #region Modifica Protocollo
        /// <summary>
        /// permette la modifica dei dati di un protocollo, se si possiedono i diritti necessari.
        /// </summary>
        /// <param name="sch"></param>
        /// <param name="infout"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        [WebMethod]
        public DocsPaVO.documento.SchedaDocumento modificaProtocollo(DocsPaVO.documento.SchedaDocumento sch, string userId, string password, out string ErrorMsg)
        {

            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "modificaProtocollo");
            DocsPaVO.documento.SchedaDocumento result = null;
            ErrorMsg = "";
            try
            {


                this.canaleWSAperto();
                this.UserID = userId;
                this.Password = password;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infout = this.getInfoUtenteAttuale();


                //TODO controlla diritti.
                if (sch != null && sch.accessRights != null && sch.accessRights != "")
                {
                    if (HMDiritti(sch.accessRights))
                    {
                        ErrorMsg = "Attenzione, l'utente " + infout.userId + " non possiede i diritti necessari per la modifica del protocollo.";
                        throw new Exception(ErrorMsg);

                    }
                }
                bool uffref = false;
                result = this.WS.DocumentoSaveDocumento(null, infout, sch, false, out uffref);

            }
            catch (Exception e)
            {

                throw e;
            }

            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }

            return result;
        }

        private static bool HMDiritti(string accessRights)
        {
            bool disabilita = false;
            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();
            if ((Convert.ToInt32(accessRights) < HMD.HMdiritti_Write)
                || (Convert.ToInt32(accessRights) < HMD.HMdiritti_Eredita))
            {
                disabilita = true;
            }
            return disabilita;
        }
        #endregion

        #region Metodi di interfacciamento al WS standard

        private DocsPaVO.utente.UserLogin.LoginResult loginOnDocspa(DocsPaVO.utente.UserLogin objLogin, bool forzaLogin)
        {
            this.Utente = null;
            this.IdAddress = null;
            return this.WS.Login(objLogin, out this.Utente, forzaLogin, this.SessionIdWSLite, out this.IdAddress);
        }

        private bool Login(string UID, string PSW)
        {

            bool retValue = false;
            this.Ruolo = null;
            try
            {

                string idAmm;
                string returnMsg;
                #region carica prima amministrazione valida

                ArrayList amministrazioni = null;
                amministrazioni = this.WS.amministrazioneGetAmministrazioni(out returnMsg);
                if ((amministrazioni == null) || (amministrazioni.Count == 0))
                    throw (new Exception("Amministrazione non valida"));

                DocsPaVO.utente.Amministrazione Amm = (DocsPaVO.utente.Amministrazione)amministrazioni[0];

                idAmm = Amm.systemId;
                #endregion carica prima amministrazione valida


                #region login
                DocsPaVO.utente.UserLogin objLogin = new DocsPaVO.utente.UserLogin();
                objLogin.UserName = UserID;
                objLogin.Password = Password;
                objLogin.IdAmministrazione = idAmm;
                objLogin.Update = false;

                //prova a loggarsi come un utente
                DocsPaVO.utente.UserLogin.LoginResult Lr = this.loginOnDocspa(objLogin, false);
                System.Text.StringBuilder descErr = new System.Text.StringBuilder("Errore in fase di Autenticazione a DocsPa: ", 150);

                if (Lr.Equals(DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN))
                {
                    descErr.Append(" Utente già connesso. ");

                    //riprova forzando l'autenticazione
                    Lr = this.loginOnDocspa(objLogin, true);

                }
                switch (Lr)
                {
                    case DocsPaVO.utente.UserLogin.LoginResult.OK:
                        {
                            descErr.Remove(0, descErr.Length);
                            retValue = true;
                            break;
                        }
                    case DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR:
                        {
                            descErr.Append(" Errore generico di applicazione.");
                            retValue = false;
                            break;
                        }
                    case DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER:
                        {
                            descErr.Append(" Utente non abilitato.");
                            retValue = false;
                            break;
                        }
                    case DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER:
                        {
                            descErr.Append(" Utente non abilitato.");
                            retValue = false;
                            break;
                        }
                    case DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN:
                        {
                            descErr.Append(" Impossibile autenticare l'utente.");
                            retValue = false;
                            break;
                        }
                    default:
                        {
                            descErr.AppendFormat(" Nessun dettaglio sull' errore {0}.", Lr.ToString());
                            retValue = false;
                            break;
                        }

                }
                if (retValue == false)
                {
                    throw (new Exception(descErr.ToString()));
                }

                #endregion login

                if (this.Utente == null)
                    throw (new Exception("Utente non valido"));

                // verifica idruolo e carica ruolo
                if (this.Utente.ruoli == null)
                {
                    throw (new Exception("Ruolo non valido"));
                }
                int i = 0;
                this.Ruolo = (DocsPaVO.utente.Ruolo)this.Utente.ruoli[i];

                retValue = true;

            }
            catch (System.SystemException ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                retValue = false;
            }
            return retValue;
        }


        private bool Logoff()
        {
            //			true  - Logoff effettuato
            //			false - errore
            bool retValue = false;

            try
            {
                this.verificaUtente();
                this.WS.Logoff(this.Utente.userId, this.Utente.idAmministrazione, this.SessionIdWSLite, this.Utente.dst);
                retValue = true;

            }
            catch (System.SystemException ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                retValue = false;
            }

            return retValue;

        }


        private bool insertCorrEsterno(string Codice, string Codrubrica,
            string Descrizione, string Tipo_urp, string Email, string Indirizzo, string Cap, string Citta, string Prov,
            string Cognome, string Nome)
        {

            #region new
            DocsPaVO.utente.Corrispondente resultCorr = null;
            DocsPaVO.utente.Corrispondente newCorr = null;

            DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtente(this.Utente, this.Ruolo);
            string idRegistro = this.getRegistroByRuolo(this.Ruolo).systemId;

            try
            {
                switch (Tipo_urp)
                {
                    case "U":
                        {
                            newCorr = new DocsPaVO.utente.UnitaOrganizzativa();
                            break;
                        }
                    case "P":
                        {
                            newCorr = new DocsPaVO.utente.Utente();
                            ((DocsPaVO.utente.Utente)newCorr).cognome = Cognome;
                            ((DocsPaVO.utente.Utente)newCorr).nome = Nome;
                            break;
                        }
                    default:
                        break;
                }

                newCorr.codiceCorrispondente = Codice;
                newCorr.codiceRubrica = Codrubrica;
                newCorr.descrizione = Descrizione;
                newCorr.tipoCorrispondente = Tipo_urp;
                newCorr.email = Email;

                #region dati costanti
                newCorr.idAmministrazione = infoUtente.idAmministrazione;
                newCorr.idRegistro = idRegistro;
                newCorr.codiceAmm = "";
                newCorr.codiceAOO = "";
                //dati canale
                DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();
                canale.systemId = "2"; //EMAIL
                newCorr.canalePref = canale;

                /*massimo digregorio: 
                 * necessari	sulla 2.0...
                 * sulla 3 non ho trovato nulla in riferimento, quindi non li gestisco
                                newCorr.tipoIE = "E";
                                newCorr.tipoCorrispondente = "S";
                */

                #endregion dati costanti

                //dati dettagli corrispondente
                DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                dettagli.Corrispondente.AddCorrispondenteRow(
                    Indirizzo,
                    Citta,
                    Cap,
                    Prov,
                    "", //nazionalita
                    "", //txt_telefono1
                    "", //txt_telefono2
                    "", //txt_fax
                    "", //txt_codfisc
                    "", //txt_note
                    "", //località
                    "", //luogoNascita
                    "", //dataNascita
                    "", //titolo
                    //"",
                    ""//codIpa
                    );
                newCorr.info = dettagli;
                newCorr.dettagli = true;

                //eseguo l'inserimento
                resultCorr = this.WS.AddressbookInsertCorrispondente(newCorr, null, infoUtente);
            }
            catch (Exception e)
            {
                throw new Exception("Errore in inserimento corrispondente esterno - DocsPaWSLite.asmx ERRORE: " + e.Message);
            }
            if (resultCorr != null && resultCorr.errore == null)
                return true;
            else
                return false;

            #endregion new


        }

        #endregion

        #region Metodi di supporto

        private bool canaleWSAperto()
        {
            if (this.WS == null)
                apriCanaleWS();

            return (this.WS != null);
        }

        private void apriCanaleWS()
        {
            this.WS = new DocsPaWS.DocsPaWebService();
            //massimo			this.machineName=DocsPaWS.DocsPaWebService.MachineName;

        }

        private void chiudiCanaleWS()
        {
            this.WS.Dispose();
        }


        private void verificaUtente()
        {

            if (this.Utente == null)
                throw (new Exception("Utente non valido"));
        }

        private void verificaRuolo()
        {
            if (this.Ruolo == null)
                throw (new Exception("oggetto Ruolo non valido"));
        }

        private DocsPaVO.utente.InfoUtente getInfoUtenteAttuale()
        {
            this.verificaUtente();
            this.verificaRuolo();

            return this.getInfoUtente(this.Utente, this.Ruolo);
        }

        private DocsPaVO.utente.InfoUtente getInfoUtente(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();

            infoUtente.idCorrGlobali = ruolo.systemId;
            infoUtente.idPeople = utente.idPeople;
            infoUtente.idGruppo = ruolo.idGruppo;
            infoUtente.dst = utente.dst;
            infoUtente.idAmministrazione = utente.idAmministrazione;
            infoUtente.userId = utente.userId;

            return infoUtente;
        }

        #region Metodi per le trasmissioni
        private ArrayList queryUtenti(DocsPaVO.utente.Corrispondente corr)
        {
           


            //costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = getInfoUtente(this.Utente, this.Ruolo).idAmministrazione;

            //corrispondenti interni
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

            return this.WS.AddressbookGetListaCorrispondenti(qco);
        }

        private DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm)
        {
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

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                //DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr);
                ArrayList listaUtenti = queryUtenti(corr);
                if (listaUtenti.Count == 0)
                    trasmissioneSingola = null;

                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = this.Ruolo;

                //				DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO (page, (DocsPaWR.UnitaOrganizzativa) corr, UserManager.getInfoUtente(page));
                //DocsPaVO.utente.Ruolo[] ruoli = this.WS.AddressbookGetRuoliRiferimentoAutorizzati(qca, theUo);
                ArrayList ruoli = this.WS.AddressbookGetRuoliRiferimentoAutorizzati(qca, theUo);
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                //trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
            /*
            else
            {
                // In questo caso questa trasmissione non può avvenire perché la
                // struttura non ha utenti (TICKET #1608)
                trasm_strutture_vuote.Add (String.Format ("{0} ({1})", corr.descrizione, corr.codiceRubrica));

            }
            */
            return trasmissione;

        }

        public DocsPaVO.documento.InfoDocumento getInfoDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();

            //infoDoc.idProfile = schedaDocumento.docNumber;//schedaDocumento.systemId;
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
                    infoDoc.mittDest = new ArrayList();
                    DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                    if (pe != null && pe.mittente != null && infoDoc.mittDest != null && infoDoc.mittDest.Count > 0)
                    {
                        //infoDoc.mittDest[0] = pe.mittente.descrizione;
                        infoDoc.mittDest.Add(pe.mittente.descrizione);
                    }
                }
                else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
                {
                    DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                    if (pu.destinatari != null)
                    {
                        infoDoc.mittDest = new ArrayList();
                        for (int i = 0; i < pu.destinatari.Count; i++)
                            //infoDoc.mittDest[i] = ((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione;
                            infoDoc.mittDest.Add((DocsPaVO.utente.Corrispondente)pu.destinatari[i]);
                    }
                }

            }
            else
            {
                infoDoc.dataApertura = schedaDocumento.dataCreazione;
            }

            return infoDoc;
        }
        #endregion

        private DocsPaVO.documento.SchedaDocumento creaSchedaDocumento(string Oggetto, string note, DocsPaVO.utente.Corrispondente corr, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {

            if (schedaDoc == null)
                throw (new Exception("Errore nella fase di inizializzazione del nuovo documento"));

            #region campi scheda costanti
            schedaDoc.systemId = null;
            schedaDoc.privato = "0";  //doc non privato

            // campi obbligatori per DocsFusion
            schedaDoc.idPeople = this.Utente.idPeople;
            schedaDoc.userId = this.Utente.userId;
            schedaDoc.typeId = "LETTERA";
            schedaDoc.appId = "ACROBAT";
            #endregion campi scheda costanti

            #region carica registro
            schedaDoc.registro = getRegistroByRuolo(this.Ruolo);
            #region verifica che la protocollazione avvenga in data odierna
            switch (this.getStatoRegistro(schedaDoc.registro))
            {
                case "V": break;// = Verde -  APERTO
                case "R": // = Rosso -  CHIUSO
                    {
                        throw (new Exception("Stato del registro non valido"));
                    }
                case "G":// = Giallo - APERTO IN GIALLO 
                    {
                        throw (new Exception("Data apertura del registro non valida"));
                    }
                default:
                    {
                        throw (new Exception("Stato del registro non valido"));
                    }
            }
            #endregion verifica che la protocollazione avvenga in data odierna
            #endregion carica registro

            #region crea oggetto scheda
            schedaDoc.oggetto = new DocsPaVO.documento.Oggetto();
            schedaDoc.oggetto.descrizione = Oggetto.Replace("'", "''");
            #endregion crea oggetto scheda

            #region crea note scheda

            if (!string.IsNullOrEmpty(note))
            {
                DocsPaVO.utente.InfoUtente infoUtente = getInfoUtente(this.Utente, this.Ruolo);
                string idPeopleDelegato = "";
                if (infoUtente.delegato != null)
                    idPeopleDelegato = infoUtente.delegato.idPeople;
                DocsPaVO.Note.InfoNota nota = new DocsPaVO.Note.InfoNota(note.Replace("'", "''"), this.Utente.systemId, this.Ruolo.systemId, idPeopleDelegato);

                schedaDoc.noteDocumento = new System.Collections.Generic.List<DocsPaVO.Note.InfoNota>();
                schedaDoc.noteDocumento.Add(nota);
            }

            #endregion crea note scheda

            #region crea oggetto protocollo
            //protocollo di entrata

            //se l'oggetto protocollo è vuoto, significa che sto invocando il metodo dal vecchio WS _lite, che 
            //elaborava solo protocolli in ingressi, vicersa l'oggetto protocolla è riempito dal chimante.
            if (schedaDoc.protocollo == null)
            {
                schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;

            }
            if (schedaDoc.registro.dataApertura != null)
                schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura;
            else
                throw (new Exception("data ultimo protocollo del registro selezionato non è settata"));
            #endregion crea oggetto protocollo

            return schedaDoc;

        }


        //Carica Registro Predefinito 
        private DocsPaVO.utente.Registro getRegistroByRuolo(DocsPaVO.utente.Ruolo objRuolo)
        {
            if (objRuolo.registri == null)
                throw (new Exception("Nessun Registro associato"));

            return (DocsPaVO.utente.Registro)objRuolo.registri[0];

        }

        private string getStatoRegistro(DocsPaVO.utente.Registro reg)
        {
            /*retValue:
            R = Rosso -  CHIUSO
            V = Verde -  APERTO
            G = Giallo - APERTO IN GIALLO
            */
            string dataApertura = reg.dataApertura;

            if (!dataApertura.Equals(""))
            {

                DateTime dt_cor = DateTime.Now;

                CultureInfo ci = new CultureInfo("it-IT");

                string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

                DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
                d_ap = d_ap.AddDays(1);

                string mydate = dt_cor.ToString(ci);

                string statoAperto = "A";
                if (reg.stato.Equals(statoAperto))
                {
                    if (dt_cor.CompareTo(d_ap) > 0)
                    {
                        //data odierna maggiore della data di apertura del registro
                        return "G";
                    }
                    else
                        return "V";
                }
            }
            return "R";

        }


        private bool ValidateEmail(string email)
        {
           

            string regex = @"^((\&quot;[^\&quot;\f\n\r\t\v\b]+\&quot;)|([\w\!\#\$\%\&amp;\'\*\+\-\~\/\^\`\|\{\}]+(\.[\w\!\#\$\%\&amp;\'\*\+\-\~\/\^\`\|\{\}]+)*))@((\[(((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9])))\])|(((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9])))|((([A-Za-z0-9\-])+\.)+[A-Za-z\-]+))$";
            System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);

            Match m = reg.Match(email);
            return m.Success;
        }

        #endregion

        #region Trasmissioni
        [WebMethod]
        public virtual bool executeTrasm(string serverPath, DocsPaVO.documento.SchedaDocumento scheda, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, string userId, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "executeTrasm");

            try
            {
                this.canaleWSAperto();

                this.UserID = userId;
                this.Password = password;

                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infout = this.getInfoUtenteAttuale();
                DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                //trasmissione.infoDocumento = DocumentManager.getInfoDocumento(scheda);
                trasmissione.infoDocumento = getInfoDocumento(scheda);


                trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infout.idPeople);
                trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infout.idCorrGlobali);


                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                        //DocsPaVO.utente.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this,mittDest.VAR_COD_RUBRICA,DocsPaWR.AddressbookTipoUtente.INTERNO);
                        DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, infout);

                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM);
                    }
                }
                trasmissione = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
                BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverPath, trasmissione);
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
        }

        [WebMethod]
        public virtual ArrayList getModelliPerTrasm(string userId, string password, DocsPaVO.utente.Registro[] registri, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "getModelliPerTrasm");

            try
            {
                this.canaleWSAperto();

                this.UserID = userId;
                this.Password = password;

                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }

                DocsPaVO.utente.InfoUtente infout = this.getInfoUtenteAttuale();
                string idAmm = infout.idAmministrazione;
                string idPeople = infout.idPeople;
                string idCorrGlobali = infout.idCorrGlobali;

                //Recupero tutti i modelli di trasmissione che l'utente può utilizzare
                ArrayList modelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, cha_tipo_oggetto, false);
                ArrayList modelliCompleti = new ArrayList();
                for (int i = 0; i < modelli.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(idAmm, ((DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)modelli[i]).SYSTEM_ID.ToString());
                    if (mod != null)
                        modelliCompleti.Add(mod);
                }
                return modelliCompleti;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
        }
        #endregion

        #region new_method

        [WebMethod]
        public virtual bool ricercaCorrLiteIE(CorrLite corrispondente, string User_ID, string Pwd, string TipoIE, out DocsPaWSLite.CorrLite corrLiteOut)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "ricercaCorrLiteIE");

            bool esito = false;
            try
            {

                this.canaleWSAperto();

                this.UserID = User_ID;
                this.Password = Pwd;

                if (!Login(this.UserID, this.Password))
                    throw (new Exception("Errore durante il login dell'utente"));

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();

                DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();

                corrLiteOut = new CorrLite();

                corr = ricercaCorrIE(corrispondente, TipoIE, infoUtente);
                //mod fly
                if ((corr != null) && (corr.systemId != null))
                {
                    esito = true;
                    if ((corr.descrizione != null) && (corr.descrizione != ""))
                    {
                        corrLiteOut.descrizione = corr.descrizione;
                    }
                    if ((corr.codiceRubrica != null) && (corr.codiceRubrica != ""))
                    {
                        corrLiteOut.codice = corr.codiceRubrica;
                    }

                    if (TipoIE == "E") corrLiteOut.tipoCorrispondente = "M";
                    else corrLiteOut.tipoCorrispondente = "D";
                    //aggiungo i dettagli
                    if (corr.info != null)
                    {

                        corrLiteOut.indirizzo = corr.info.Tables[0].Rows[0][0].ToString();
                        corrLiteOut.citta = corr.info.Tables[0].Rows[0][1].ToString();
                        corrLiteOut.cap = corr.info.Tables[0].Rows[0][2].ToString();
                        corrLiteOut.provincia = corr.info.Tables[0].Rows[0][3].ToString();
                        corrLiteOut.nazione = corr.info.Tables[0].Rows[0][4].ToString();
                        corrLiteOut.telefono = corr.info.Tables[0].Rows[0][5].ToString();
                        corrLiteOut.telefono2 = corr.info.Tables[0].Rows[0][6].ToString();
                        corrLiteOut.codiceFiscale = corr.info.Tables[0].Rows[0][8].ToString();
                        corrLiteOut.note = corr.info.Tables[0].Rows[0][9].ToString();


                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
            return esito;
        }

        [WebMethod]
        public virtual DocsPaVO.documento.Allegato aggiungiAllegato(string User_ID, string Pwd, DocsPaVO.documento.SchedaDocumento scheda, string NumPagine, string Descrizione, DocsPaVO.documento.FileDocumento fd, DocsPaVO.documento.Allegato allegato)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "aggiungiAllegato");
          
            bool esito = false;
            string errorMessage = String.Empty;
            try
            {

                this.canaleWSAperto();
                this.UserID = User_ID;
                this.Password = Pwd;

                if (!Login(this.UserID, this.Password))
                    throw (new Exception("Errore durante il login dell'utente"));

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();

                allegato.descrizione = Descrizione;
                allegato.numeroPagine = Convert.ToInt32(NumPagine);
                allegato.docNumber = scheda.docNumber;
                allegato.version = "0";
                allegato = this.WS.DocumentoAggiungiAllegato(infoUtente, allegato);
                DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)allegato;
                this.WS.DocumentoPutFileNoException(ref fr, fd, infoUtente, out errorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
            return allegato;

        }

        [WebMethod]
        public virtual DocsPaVO.documento.FileDocumento aggiungiEtichettaPDF(DocsPaVO.documento.FileRequest fr, DocsPaVO.documento.SchedaDocumento scheda, string position, string userID, string password, DocsPaVO.documento.FileDocumento fd)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "aggiungiEtichettaPDF");

            try
            {

                this.canaleWSAperto();

                this.UserID = userID;
                this.Password = password;

                if (!Login(this.UserID, this.Password))
                    throw (new Exception("Errore durante il login dell'utente"));

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();

                string xmlPath = this.Server.MapPath("XML/labelPdf.xml");

                fd = BusinessLogic.Documenti.FileManager.setFileConSegnatura(fr, scheda, infoUtente, xmlPath, position, ref fd);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
            return fd;

        }

        [WebMethod]
        public virtual DocsPaVO.documento.FileDocumento getFileAllegato(DocsPaVO.documento.FileRequest allegato, string userID, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "getFileAllegato");
            DocsPaVO.documento.FileDocumento fileDoc = null;

            try
            {
                this.canaleWSAperto();
                this.UserID = userID;
                this.Password = password;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUt = this.getInfoUtenteAttuale();

                if (allegato != null)
                {
                    fileDoc = this.WS.DocumentoGetFile(allegato, infoUt);
                }
                return fileDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
        }

        [WebMethod]
        public virtual bool ricercaCorrLiteIEbyDescrizione(string descRicerca, string User_ID, string Pwd, string tipoIE, out DocsPaWSLite.CorrLite[] corrLiteOut)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "ricercaCorrLiteIEbyDescrizione");
            
            bool esito = false;
            corrLiteOut = null;
            try
            {

                this.canaleWSAperto();
                this.UserID = User_ID;
                this.Password = Pwd;

                if (!Login(this.UserID, this.Password))
                    throw (new Exception("Errore durante il login dell'utente"));

                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();

                ArrayList result = new ArrayList();
                result = WS.AddressbookGetListaCorrispondentiLite(descRicerca, tipoIE, infoUtente.idAmministrazione);

                if (result.Count > 0)
                {
                    corrLiteOut = new CorrLite[result.Count];

                    for (int i = 0; i < result.Count; i++)
                    {
                        CorrLite corr = new CorrLite();
                        corr.descrizione = ((DocsPaVO.utente.Corrispondente)result[i]).descrizione.ToString();
                        corr.codice = ((DocsPaVO.utente.Corrispondente)result[i]).codiceRubrica.ToString();
                        if (tipoIE == "E") corr.tipoCorrispondente = "M"; else corr.tipoCorrispondente = "D";

                        if (tipoIE == "E")
                        {
                            DocsPaVO.addressbook.TipoUtente tu = DocsPaVO.addressbook.TipoUtente.INTERNO;
                            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                            qco.systemId = ((DocsPaVO.utente.Corrispondente)result[i]).systemId;
                            ((DocsPaVO.utente.Corrispondente)result[i]).info = this.WS.AddressbookGetDettagliCorrispondente(qco);

                            if (((DocsPaVO.utente.Corrispondente)result[i]).info != null)
                            {
                                corr.indirizzo = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][0].ToString();
                                corr.citta = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][1].ToString();
                                corr.cap = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][2].ToString();
                                corr.provincia = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][3].ToString();
                                corr.nazione = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][4].ToString();
                                corr.telefono = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][5].ToString();
                                corr.telefono2 = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][6].ToString();
                                corr.codiceFiscale = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][8].ToString();
                                corr.note = ((DocsPaVO.utente.Corrispondente)result[i]).info.Tables[0].Rows[0][9].ToString();
                            }
                        }
                        corrLiteOut[i] = corr;
                        esito = true;

                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
            return esito;

        }


        private bool TrasmissioneExecuteTrasmDocDaModello(DocsPaVO.utente.InfoUtente infoUtente, string serverPath, DocsPaVO.documento.SchedaDocumento scheda, string modelloTrasmissione)
        {
           

            try
            {

                ArrayList lista = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliByName(infoUtente.idAmministrazione, modelloTrasmissione, "D", this.Ruolo.idRegistro, infoUtente.idPeople, this.Ruolo.systemId);


                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                bool trovato = false;

                if (lista != null && lista.Count > 0)
                {
                    for (int i = 0; i < lista.Count && !trovato; i++)
                    {
                        modello = (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)lista[i];
                        if (modello.NOME.ToUpper() == modelloTrasmissione.ToUpper())
                        {
                            trovato = true;
                            break;
                        }
                    }
                }

                if (!trovato)
                {
                    return false;
                }
                DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                //trasmissione.infoDocumento = DocumentManager.getInfoDocumento(scheda);
                trasmissione.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(scheda);


                trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);


                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                        //DocsPaVO.utente.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this,mittDest.VAR_COD_RUBRICA,DocsPaWR.AddressbookTipoUtente.INTERNO);
                        DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, infoUtente);

                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM);
                    }
                }
                if (infoUtente.delegato != null)
                    trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

                // BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverPath, trasmissione);
                trasmissione = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverPath, trasmissione);

                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }

        /// <summary>
        /// permette la creazione di una versione (record nel DB e contenuto).
        /// </summary>
        [WebMethod]
        public DocsPaVO.documento.FileRequest aggiungiVersione(DocsPaVO.documento.FileRequest fileReq, DocsPaVO.documento.FileDocumento fileDoc, string userId, string password)
        {
            ServiceLoadController.LoadControllerManager.CheckInterval("DocsPaWSLite", "addVersion");
            DocsPaVO.documento.FileRequest fileReqOut = null;
            try
            {
                this.canaleWSAperto();
                this.UserID = userId;
                this.Password = password;
                if (!Login(this.UserID, this.Password))
                {
                    throw (new Exception("Errore durante il login dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtenteAttuale();
                //controllo sulla preseza delle informazioni principali:
                //docNumber del documento in fileRequest
                //nel FileDocumento controllare la presenza del content e delle informazioni necesssarie (non avrebbe senzo creare una versione senza poi associare il file
                if (fileReq == null || fileDoc == null)
                {
                    throw (new Exception("E' necessario indicare tutti gli oggetti"));
                }
                if (string.IsNullOrEmpty(fileDoc.contentType) && string.IsNullOrEmpty(fileDoc.estensioneFile))
                {
                    throw (new Exception("E' necessario specificare estensione del file e contentType"));
                }
                if (fileDoc.content == null)
                {
                    throw (new Exception("E' necessario inserire il contenuto del file"));
                }


                //valori da aggiungere
                fileReq.version = "!";

                DocsPaVO.documento.FileRequest fileReq1 = null;

                bool esito = false;
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext(IsolationLevel.Serializable))
                {

                    try
                    {

                        //1° addVersion
                        fileReq1 = BusinessLogic.Documenti.VersioniManager.addVersion(fileReq, infoUtente, false);
                        if (fileReq1 != null && !string.IsNullOrEmpty(fileReq1.versionId))
                        {
                            //2° putfile

                            string outMessage;
                            //esito = BusinessLogic.Documenti.FileManager.putFile(ref fileReq1, fileDoc, infoUtente, false, out outMessage);
                            fileReqOut = BusinessLogic.Documenti.FileManager.putFile(fileReq, fileDoc, infoUtente);
                            if (fileReqOut != null && !string.IsNullOrEmpty(fileReqOut.versionId))
                            //if (esito)
                            {
                                // infoUtente.codWorkingApplication = "wsLite";
                                //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", fileReq.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunta al N.ro Doc.: ", fileReq.docNumber, " la Ver. ", fileReq.version), DocsPaVO.Logger.CodAzione.Esito.OK);
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", fileReq.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunta al N.ro Doc.: ", fileReq.docNumber, " la Ver. ", fileReq.version), DocsPaVO.Logger.CodAzione.Esito.OK);
                                transactionContext.Complete();
                                //return fileReq1;
                                return fileReqOut;
                            }

                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw (new Exception("Si è verificato un errore nella creazione della versione"));
                        return null;
                    }

                    transactionContext.Dispose();
                    return null;
                }

            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                this.Logoff();
                this.chiudiCanaleWS();
            }
        }

        #endregion
    }

}


