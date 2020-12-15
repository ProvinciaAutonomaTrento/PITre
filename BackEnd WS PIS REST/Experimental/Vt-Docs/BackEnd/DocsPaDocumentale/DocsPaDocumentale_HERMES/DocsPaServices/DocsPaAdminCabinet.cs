using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.utente;
using System.Globalization;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaServices
{
    /// <summary>
    /// Classe per il reperimento delle informazioni relative al cabinet documentum,
    /// che corrisponde ad un'amministrazione di docspa
    /// </summary>
    public sealed class DocsPaAdminCabinet
    {
        /// <summary>
        /// Costante che identifica il nome dei folder per tutti i documenti
        /// </summary>
        public const string FOLDER_DOCUMENTI = "Documenti";

        /// <summary>
        /// Costante che identifica il nome dei folder che contiene i report generati da docspa
        /// </summary>
        public const string FOLDER_REPORT = "Report";

        /// <summary>
        /// Costante che identifica il nome dei folder che contiene i titolari di classificazione
        /// </summary>
        public const string FOLDER_TITOLARIO = "Titolario";

        /// <summary>
        /// Reperimento nome folder dctm per i documenti
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getRootDocumenti(InfoUtente infoUtente)
        {
            return getRootDocumenti(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Reperimento root folder per tutti i documenti
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getRootDocumenti(string codiceAmministrazione)
        {
            return string.Concat("/", codiceAmministrazione.ToLowerInvariant(), "/", FOLDER_DOCUMENTI);
        }

        /// <summary>
        /// Reperimento cartella corrente per il repository dei documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="dataCreazione"></param>
        /// <returns></returns>
        public static string getPathDocumento(InfoUtente infoUtente, string dataCreazione)
        {
            return getPathDocumento(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione), dataCreazione);
        }

        /// <summary>
        /// Reperimento cartella corrente per il repository dei documenti
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="dataCreazione"></param>
        /// <returns></returns>
        public static string getPathDocumento(string codiceAmministrazione, string dataCreazione)
        {
            DateTime date = Convert.ToDateTime(dataCreazione, new CultureInfo("it-IT"));

            return string.Format("/{0}/{1}/{2}/{3}/", 
                codiceAmministrazione.ToLowerInvariant(), 
                DocsPaAdminCabinet.FOLDER_DOCUMENTI,
                date.Year.ToString(),
                date.Month.ToString());
        }

        /// <summary>
        /// Reperimento nome folder dctm per i report
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getRootReport(InfoUtente infoUtente)
        {
            return getRootReport(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Reperimento nome folder dctm per i documenti di tipo stampa registro
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getRootReport(string codiceAmministrazione)
        {
            return string.Format("/{0}/{1}/", codiceAmministrazione.ToLowerInvariant(), FOLDER_REPORT);
        }

        /// <summary>
        /// Reperimento path per il folder della stampa del registro richiesto
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static string getPathStampaRegistro(InfoUtente infoUtente, string idRegistro)
        {
            return getPathStampaRegistro(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione), idRegistro);
        }

        /// <summary>
        /// Reperimento path per il folder della stampa del registro richiesto
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static string getPathStampaRegistro(string codiceAmministrazione, string idRegistro)
        {
            // Reperimento codice registro
            string codiceRegistro = DocsPaQueryHelper.getCodiceRegistro(idRegistro);

            return string.Format("{0}/{1}/{2}/", getRootReport(codiceAmministrazione), "Registro di protocollo", codiceRegistro);
        }

        /// <summary>
        /// Reperimento nome folder dctm per i titolari di classificazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string getRootTitolario(InfoUtente infoUtente)
        {
            return getRootTitolario(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Reperimento nome folder dctm per i titolari di classificazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getRootTitolario(string codiceAmministrazione)
        {
            return string.Concat("/", codiceAmministrazione.ToLowerInvariant(), "/", FOLDER_TITOLARIO);
        }

        /// <summary>
        /// Restituisce il root path associato ad una amministrazione (che è un cabinet nella fattispecie)
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string getRootAmministrazione(InfoUtente infoUtente)
        {
            return getRootAmministrazione(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Restituisce il root path associato ad una amministrazione (che è un cabinet nella fattispecie)
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getRootAmministrazione(string codiceAmministrazione)
        {
            return string.Concat("/", codiceAmministrazione.ToLowerInvariant());
        }

        /// <summary>
        /// Reperimento del codice dell'amministrazione richiesta
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string getCodiceAmministrazione(InfoUtente infoUtente)
        {
            return getCodiceAmministrazione(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Reperimento del codice dell'amministrazione richiesta
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getCodiceAmministrazione(string codiceAmministrazione)
        {
            return codiceAmministrazione.ToLowerInvariant();
        }
    }
}