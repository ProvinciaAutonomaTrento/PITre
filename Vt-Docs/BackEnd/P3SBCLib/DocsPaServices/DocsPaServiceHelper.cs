using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using DocsPaVO.ricerche;

namespace P3SBCLib.DocsPaServices
{
    /// <summary>
    /// Helper class per l'utilizzo dei servizi dello strato business di docspa
    /// </summary>
    internal sealed class DocsPaServiceHelper
    {
        /// <summary>
        /// Codice della funzione in amministrazione che consente
        /// di abilitare l'utilizzo dei servizi SBC agli utenti
        /// appartenenti ad un determinato ruolo
        /// </summary>
        private const string FUNZIONE_ABILITAZIONE_SERVIZI = "GEST_SERVIZI_SBC";

        /// <summary>
        /// 
        /// </summary>
        private DocsPaServiceHelper() { }

        #region Public methods

        /// <summary>
        /// Task di impersonificazione utente SBC in utente DocsPa
        /// </summary>
        /// <param name="userIdRichiedente"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.InfoUtente Impersonate(string userIdRichiedente)
        {   
            // Reperimento oggetto utente richiedente
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(userIdRichiedente, GetIdAmministrazione(userIdRichiedente));

            if (utente == null)
                throw new ApplicationException(string.Format("Utente {0} non trovato", userIdRichiedente));

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, GetRuoloPreferito(utente.idPeople));

            // Reperimento token superutente
            infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

            return infoUtente;
        }

        /// <summary>
        /// Reperimento dell'id del primo ruolo di riferimento cui l'utente risulta associato
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        private static string GetIdRuoloPreferito(string idPeople)
        {
            DocsPaVO.amministrazione.OrgRuolo[] orgRuoli = (DocsPaVO.amministrazione.OrgRuolo[])BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUtente(idPeople).ToArray(typeof(DocsPaVO.amministrazione.OrgRuolo));

            DocsPaVO.amministrazione.OrgRuolo ruolo = orgRuoli.Where(e => e.DiRiferimento == "1").FirstOrDefault();

            if (ruolo != null)
                return ruolo.IDGruppo;
            else
                return string.Empty;

            //string idRuoloRiferimento = string.Empty;

            //using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            //{
            //    string commandText = string.Format("SELECT cg.id_gruppo FROM dpa_corr_globali cg INNER JOIN peoplegroups pg on cg.id_gruppo = pg.groups_system_id WHERE pg.people_system_id = {0} AND cha_riferimento = '1' AND pg.dta_fine IS NULL", idPeople);
            //    //DocsPaUtils.LogsManagement.Debugger.Write(commandText);

            //    dbProvider.ExecuteScalar(out idRuoloRiferimento, commandText);
            //}

            //return idRuoloRiferimento;
        }

        /// <summary>
        /// Reperimento del primo ruolo di appartenenza dell'utente per cui risulta abilitata la microfunzione GEST_SERVIZI_SBC
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo GetRuoloPreferito(string idPeople)
        {
            DocsPaVO.utente.Ruolo ruoloPreferito = null;

            DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));

            if (ruoli != null && ruoli.Length > 0)
            {
                foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                {
                    DocsPaVO.utente.Funzione[] funzioni = (DocsPaVO.utente.Funzione[])ruolo.funzioni.ToArray(typeof(DocsPaVO.utente.Funzione));

                    DocsPaVO.utente.Funzione funzione = funzioni.Where(e => e.codice == FUNZIONE_ABILITAZIONE_SERVIZI).FirstOrDefault();

                    if (funzione != null)
                    {
                        ruoloPreferito = ruolo;
                        break;
                    }
                }

                if (ruoloPreferito == null)
                    throw new ApplicationException("L'utente non dispone dei privilegi per l'utilizzo dei servizi SBC");
            }
            else
                throw new ApplicationException("L'utente non non risulta associato ad alcun ruolo");

            return ruoloPreferito;

            //if (ruoli != null && ruoli.Length > 0)
            //{
            //    //// Reperimento id ruolo di riferimento
            //    //string idRuoloRiferimento = GetIdRuoloPreferito(idPeople);

            //    //if (string.IsNullOrEmpty(idRuoloRiferimento))
            //    //    throw new ApplicationException("Per l'utente non risulta definito alcun ruolo preferito in amministrazione");

            //    //DocsPaVO.utente.Ruolo ruoloRif = ruoli.Where(e => e.idGruppo == idRuoloRiferimento).FirstOrDefault();

            //    //if (ruoloRif == null)
            //    //    throw new ApplicationException("Non è stato possibile reperire il ruolo preferito per l'utente");
            //    //else
            //    //    return ruoloRif;
            //}
            //else
            //    throw new ApplicationException("L'utente non non risulta associato ad alcun ruolo");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="ruoloPreferito"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Registro GetRegistroRuoloPreferito(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruoloPreferito)
        {
            if (ruoloPreferito.registri.Count == 0)
                throw new ApplicationException(string.Format("Nessun registro associato al ruolo {0} per l'utente {1}", ruoloPreferito.codice, infoUtente.userId));

            return (DocsPaVO.utente.Registro)ruoloPreferito.registri[0];
        }

        /// <summary>
        /// Reperimento registro associato al ruolo preferito
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Registro GetRegistroRuoloPreferito(DocsPaVO.utente.InfoUtente infoUtente)
        {
            // Reperimento del ruolo preferito per l'utente corrente
            DocsPaVO.utente.Ruolo ruoloPreferito = DocsPaServices.DocsPaServiceHelper.GetRuoloPreferito(infoUtente);

            return GetRegistroRuoloPreferito(infoUtente, ruoloPreferito);
        }

        /// <summary>
        /// Reperimento del ruolo preferito per l'utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo GetRuoloPreferito(DocsPaVO.utente.InfoUtente infoUtente)
        {
            return GetRuoloPreferito(infoUtente.idPeople);
        }

        /// <summary>
        /// Reperimento utenti attivi in amministrazione
        /// </summary>
        /// <param name="classificazioni"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.OrgUtente[] GetUtenti(string[] classificazioni, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            // Reperimento id amministrazione a partire dal codice classificazione (obbligatorio) inserito nei filtri di ricerca
            string idAmministrazione = GetIdAmministrazioneClassificazione(classificazioni[0]);

            return BusinessLogic.Amministrazione.OrganigrammaManager.GetUtentiNodiTitolario(idAmministrazione, classificazioni, filtriRicerca, pagingContext);
        }

        /// <summary>
        /// Reperimento documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.InfoDocumento[] GetDocumenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            int numTotPage, nRec;
            List<SearchResultInfo> idProfilesList = new List<SearchResultInfo>();
            ArrayList documenti = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(infoUtente.idGruppo,
                                    infoUtente.idPeople,
                                    new DocsPaVO.filtri.FiltroRicerca[1][] { filtriRicerca },
                                    false,
                                    pagingContext.Page,
                                    pagingContext.PageSize, 
                                    true, 
                                    out numTotPage, 
                                    out nRec,
                                    pagingContext.GetIdProfilesList,
                                    out idProfilesList,false);
            
            if (documenti == null)
                throw new ApplicationException("Errore nella ricerca dei documenti");

            pagingContext.SetRecordCount(nRec);

            // Salvataggio lista idProfile
            pagingContext.IdProfilesList = new List<string>();

            if (idProfilesList != null)
            {
                foreach (SearchResultInfo temp in idProfilesList)
                {
                    pagingContext.IdProfilesList.Add(temp.Id);
                }
            }

            return (DocsPaVO.documento.InfoDocumento[]) documenti.ToArray(typeof(DocsPaVO.documento.InfoDocumento));
        }

        /// <summary>
        /// Reperimento fascicoli
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceClassificazione"></param>
        /// <param name="ricercaInClassificazioniFiglie"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="filtriRicercaDocumenti"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoli(DocsPaVO.utente.InfoUtente infoUtente, string codiceClassificazione, bool ricercaInClassificazioniFiglie, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, DocsPaVO.filtri.FiltroRicerca[] filtriRicercaDocumenti, DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            // Lista dei system id restituiti dalla ricerca. Non utitlizzata
            List<SearchResultInfo> idProject = null;

            // Reperimento oggetto classificazione
            DocsPaVO.fascicolazione.Classificazione classificazione = GetClassificazione(infoUtente, codiceClassificazione);

            int numTotPage, nRec;
            ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(infoUtente, 
                                        classificazione, null,
                                        filtriRicerca, filtriRicercaDocumenti,
                                        false, true, ricercaInClassificazioniFiglie,
                                         out numTotPage, out nRec, pagingContext.Page, pagingContext.PageSize, false, out idProject, null, string.Empty);

            pagingContext.SetRecordCount(nRec);

            return (DocsPaVO.fascicolazione.Fascicolo[]) fascicoli.ToArray(typeof(DocsPaVO.fascicolazione.Fascicolo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceFascicolo"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder GetFolderFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo)
        {
            DocsPaVO.fascicolazione.Folder folder = BusinessLogic.Fascicoli.FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, GetFascicolo(infoUtente, codiceFascicolo));

            if (folder == null)
                throw new ApplicationException(string.Format("Folder per il fascicolo con codice {0} non trovato", codiceFascicolo));

            return folder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceFascicolo"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo)
        {
            // Reperimento registro per la ricerca fascicolo
            // DocsPaVO.utente.Registro registro = DocsPaServices.DocsPaServiceHelper.GetRegistroRuoloPreferito(infoUtente);

            DocsPaVO.fascicolazione.Fascicolo fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, codiceFascicolo, null, false, true);

            if (fascicolo == null)
                throw new ApplicationException(string.Format("Fascicolo con codice {0} non trovato", codiceFascicolo));

            if (string.IsNullOrEmpty(fascicolo.codiceGerarchia) && !string.IsNullOrEmpty(fascicolo.idClassificazione))
            {
                // Reperimento del codice del nodo titolario (codice gerarchia) di appartenenza del fascicolo, 
                // nel caso non sia stato specificato come descrizione ma solo come ID
                DocsPaVO.amministrazione.OrgTitolario titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(fascicolo.idClassificazione);
                if (titolario != null)
                    fascicolo.codiceGerarchia = titolario.Codice;
            }

            return fascicolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.InfoDocumento GetDocumento(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            DocsPaVO.documento.InfoDocumento infoDocumento = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUtente, docNumber, docNumber, true);

            if (infoDocumento == null)
                throw new ApplicationException(string.Format("Documento con id {0} non trovato", docNumber));

            return infoDocumento;
        }

        /// <summary>
        /// Reperimento oggetto scheda documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento GetSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, docNumber, docNumber);

            if (schedaDocumento == null)
                throw new ApplicationException(string.Format("Documento con id {0} non trovato", docNumber));

            return schedaDocumento;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceTipoDocumento"></param>
        /// <returns></returns>
        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateTipoDocumento(DocsPaVO.utente.InfoUtente infoUtente, string codiceTipoDocumento)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplatePerRicerca(infoUtente.idAmministrazione, codiceTipoDocumento);

            if (template == null)
                throw new ApplicationException(string.Format("Tipologia documento {0} non trovata", codiceTipoDocumento));

            return template;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceTipoFascicolo"></param>
        /// <returns></returns>
        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateTipoFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string codiceTipoFascicolo)
        {
            DocsPaVO.ProfilazioneDinamica.Templates[] templates = (DocsPaVO.ProfilazioneDinamica.Templates[])
                    BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplatesFasc(infoUtente.idAmministrazione).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.Templates));

            var t = (from c in templates where c.DESCRIZIONE == codiceTipoFascicolo select c);
         
            if (t.Count<DocsPaVO.ProfilazioneDinamica.Templates>() > 0)
                return t.First<DocsPaVO.ProfilazioneDinamica.Templates>();
            else
                throw new ApplicationException(string.Format("Tipologia fascicolo {0} non trovata", codiceTipoFascicolo));
        }

        /// <summary>
        /// Reperimento oggetto custom
        /// </summary>
        /// <param name="oggettiCustom"></param>
        /// <param name="codiceOggetto"></param>
        /// <returns></returns>
        public static DocsPaVO.ProfilazioneDinamica.OggettoCustom GetOggettoCustom(List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> oggettiCustom, string codiceOggetto)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (from c in oggettiCustom
                                                                         where c.DESCRIZIONE == codiceOggetto
                                                                         select c).FirstOrDefault<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();

            if (oggettoCustom == null)
                throw new ApplicationException(string.Format("Previsto campo {0} per la tipologia specificata", codiceOggetto));

            return oggettoCustom;
        }

        /// <summary>
        /// Impostazione valore per un campo custom di profilazione
        /// </summary>
        /// <param name="oggettiCustom"></param>
        /// <param name="codiceOggetto"></param>
        /// <param name="valore"></param>
        /// <returns></returns>
        public static void SetValoreOggettoCustom(List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> oggettiCustom, string codiceOggetto, string valore)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustomBene = GetOggettoCustom(oggettiCustom, codiceOggetto);
            oggettoCustomBene.VALORE_DATABASE = valore;
        }

        /// <summary>
        /// Reperimento della descrizione dello stato associato al documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public static string GetDescrizioneStatoDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            string descrizioneStato = string.Empty;

            if (!string.IsNullOrEmpty(infoDocumento.idTipoAtto))
            {
                DocsPaVO.DiagrammaStato.Stato stato = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(infoDocumento.docNumber);
                if (stato != null)
                    descrizioneStato = stato.DESCRIZIONE;
            }

            return descrizioneStato;
        }

        /// <summary>
        /// Reperimento dell'ultima nota visibile dall'utente associata al documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public static string GetNotaDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            return BusinessLogic.Note.NoteManager.GetUltimaNotaAsString(infoUtente, 
                new DocsPaVO.Note.AssociazioneNota { Id = infoDocumento.idProfile, TipoOggetto = DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento });
        }

        /// <summary>
        /// Reperimento file associato al documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento GetFile(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);

            DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

            if (IsAcquired(fileRequest))
                // Download del file associato al documento
                return BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
            else
                return null;
        }

        /// <summary>
        /// Reperimento del nome del file acquisito per la versione corrente del documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string GetFileName(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            return System.IO.Path.GetFileName(BusinessLogic.Documenti.FileManager.getCurrentVersionFilePath(infoUtente, docNumber));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroDocumento"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        public static void SetFile(DocsPaVO.utente.InfoUtente infoUtente, string numeroDocumento, string fileName, byte[] fileContent)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, numeroDocumento, numeroDocumento);

            DocsPaVO.documento.FileRequest fileRequest = null;

            if (schedaDocumento.documenti.Count > 0)
            {
                // Reperimento ultima versione del documento
                fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                if (fileRequest.fileSize != "0")
                {
                    // Se per l'ultima versione del documento è stato acquisito un file,
                    // viene creata nuova versione per il documento
                    fileRequest = new DocsPaVO.documento.FileRequest();
                    fileRequest.fileName = fileName;
                    fileRequest.docNumber = numeroDocumento;
                    fileRequest.descrizione = string.Empty;

                    fileRequest = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);
                }
                else
                    fileRequest.fileName = fileName;
            }

            DocsPaVO.documento.FileDocumento fileDocument = new DocsPaVO.documento.FileDocumento
            {
                name = fileName,
                estensioneFile = System.IO.Path.GetExtension(fileName).Replace(".", string.Empty),
                content = fileContent,
                length = fileContent.Length
            };
            
            fileRequest = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDocument, infoUtente);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Verifica se per l'ultima versione del documento corrente è stato acquisito un file
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private static bool IsAcquired(DocsPaVO.documento.FileRequest fileRequest)
        {
            return (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != string.Empty &&
                    fileRequest.fileSize != null && fileRequest.fileSize != "0");
        }

        /// <summary>
        /// Reperimento oggetto classificazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceClassifica"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Classificazione GetClassificazione(DocsPaVO.utente.InfoUtente infoUtente, string codiceClassifica)
        {
            ArrayList listClassificazioni = BusinessLogic.Fascicoli.TitolarioManager.getTitolario(
            infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, null, codiceClassifica, false);

            if (listClassificazioni != null && listClassificazioni.Count > 0)
                return (DocsPaVO.fascicolazione.Classificazione)listClassificazioni[0];
            else
                throw new ApplicationException(string.Format("Classificazione con codice {0} non trovata", codiceClassifica));
        }

        /// <summary>
        /// Reperimento dell'id dell'amministrazione di appartenenza dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static string GetIdAmministrazione(string userId)
        {
            string idAmministrazione = BusinessLogic.Utenti.UserManager.getIdAmmUtente(userId);

            if (string.IsNullOrEmpty(idAmministrazione))
                throw new ApplicationException(string.Format("Nessuna amministrazione trovata per l'utente {0}", userId));

            return idAmministrazione;
        }

        /// <summary>
        /// Reperimento id amministrazione a partire da un codice classificazione
        /// </summary>
        /// <param name="codiceClassificazione"></param>
        /// <returns></returns>
        private static string GetIdAmministrazioneClassificazione(string codiceClassificazione)
        {
            string idAmministrazione = BusinessLogic.Amministrazione.TitolarioManager.GetIdAmministrazione(codiceClassificazione);

            if (string.IsNullOrEmpty(idAmministrazione))
                throw new ApplicationException(string.Format("Nessuna amministrazione trovata per la classificazione con codice {0}", codiceClassificazione));

            return idAmministrazione;
        }

        /// <summary>
        /// Ricerca di un filtro in base al nome
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string FindFilterByName(DocsPaVO.filtri.FiltroRicerca[] filters, string name)
        {
            return (from c in filters
                      where c.argomento == name
                      select c.valore).First<string>();
        }

        #endregion
    }
}
