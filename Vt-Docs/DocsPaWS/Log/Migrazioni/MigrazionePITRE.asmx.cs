using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace DocsPaWS.Migrazioni
{
    /// <summary>
    /// WebService di migrazione dei dati DocsPa in PITRE
    /// </summary>
    [WebService(Namespace = "http://valueteam.it/MigrazionePITRE", 
        Description = "WebService di migrazione dei dati DocsPa in PITRE")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class MigrazionePITRE : System.Web.Services.WebService
    {
        /// <summary>
        /// Reperimento delle amministrazioni in PITRE
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "Reperimento delle amministrazioni in PITRE")]
        public DocsPaVO.amministrazione.InfoAmministrazione[] GetAmministrazioni()
        {
            return DocsPaDocumentale_PITRE.Migrazione.Amministrazione.GetAmministrazioni();
        }

        /// <summary>
        /// Lettura log di migrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="flush"></param>
        /// <returns></returns>
        [WebMethod(Description = "Lettura log di migrazione")]
        public string GetLogMigrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, bool flush)
        {
            string log = DocsPaDocumentale_PITRE.Migrazione.Log.GetInstance(amministrazione).ReadAll();

            if (flush)
                DocsPaDocumentale_PITRE.Migrazione.Log.Delete(amministrazione);

            return log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public string GetLogStatoCorrente(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Log.GetInstance(amministrazione).ReadCurrent();
        }

        /// <summary>
        /// Reperimento delle informazioni riguardanti lo stato della migrazione dei dati per l'amministrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        [WebMethod(Description = "Reperimento delle informazioni riguardanti lo stato della migrazione dei dati per l'amministrazione")]
        public DocsPaDocumentale_PITRE.Migrazione.InfoStatoMigrazione GetStatoMigrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            return DocsPaDocumentale_PITRE.Migrazione.StatoMigrazione.Get(amministrazione);
        }

        /// <summary>
        /// Forza la scadenza delle password di tutti gli utenti dell'amministrazione come scadute
        /// </summary>
        /// <param name="amministrazione"></param>
        [WebMethod(Description = "Forza la scadenza delle password di tutti gli utenti dell'amministrazione come scadute")]
        public void ForzaScadenzaPassword(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Amministrazione.ForzaScadenzaPassword(amministrazione);
        }

        /// <summary>
        /// Migrazione di un'amministrazione in PITRE
        /// </summary>
        /// <param name="amministrazioni"></param>
        [WebMethod(Description = "Migrazione di un'amministrazione in PITRE")]
        public bool ImportaAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Amministrazione.ImportaAmministrazione(amministrazione);
        }

        /// <summary>
        /// Rimozione di un'amministrazione in DCTM
        /// </summary>
        /// <param name="amministrazioni"></param>
        [WebMethod(Description = "Rimozione di un'amministrazione in DCTM")]
        public bool RimuoviAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Amministrazione.RimuoviAmministrazione(amministrazione);
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione in DCTM
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        [WebMethod(Description = "Aggiornamento di un'amministrazione in DCTM")]
        public bool AggiornaAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.Amministrazione.TipiOggettiAmministrazione tipiOggetti)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Amministrazione.AggiornaAmministrazione(amministrazione, tipiOggetti);
        }

        /// <summary>
        /// Interruzione import fascicoli
        /// </summary>
        /// <param name="amministrazione"></param>
        [WebMethod()]
        public void InterrompiImportaFascicoli(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.Interrompi(amministrazione);
        }

        /// <summary>
        /// Migrazione in DCTM di tutti i fascicoli di un'amministrazioni PITRE
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        [WebMethod(Description = "Migrazione in DCTM di tutti i fascicoli di un'amministrazioni PITRE")]
        public void ImportaFascicoli(DocsPaVO.amministrazione.InfoAmministrazione amministrazione,
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.OpzioniMigrazioneFascicolo opzioniMigrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.ImportaFascicoli(amministrazione, opzioniMigrazione);
        }

        /// <summary>
        /// Migrazione in DCTM di tutti i fascicoli selezionati di un'amministrazioni PITRE
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="fascicoli"></param>
        [WebMethod(Description = "Migrazione in DCTM di tutti i fascicoli selezionati di un'amministrazioni PITRE")]
        public void ImportaFascicoliSelezionati(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.InfoFascicoloMigrazione[] fascicoli)
        {
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.ImportaFascicoliSelezionati(amministrazione, fascicoli);
        }

        /// <summary>
        /// Aggiornamento delle associazioni fascicoli / documenti
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="riprendiDaIdFascicolo"></param>
        /// <param name="fascicoli"></param>
        [WebMethod(Description = "Aggiornamento delle associazioni fascicoli / documenti")]
        public void AggiornaAssociazioniFascicoliDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, 
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.OpzioniMigrazioneFascicolo opzioniMigrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.AggiornaAssociazioniFascicoliDocumenti(amministrazione, opzioniMigrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="fascicoli"></param>
        [WebMethod(Description = "Aggiornamento delle associazioni fascicoli / documenti")]
        public void AggiornaAssociazioniFascicoliSelezionatiDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.InfoFascicoloMigrazione[] fascicoli)
        {
            DocsPaDocumentale_PITRE.Migrazione.Fascicolo.AggiornaAssociazioniFascicoliSelezionatiDocumenti(amministrazione, fascicoli);
        }

        /// <summary>
        /// Reperimento dei filtri validi e applicabili alla migrazione dei documenti in DCTM
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="filtro"></param>
        /// <param name="numeroDocumenti"></param>
        /// <returns></returns>
        [WebMethod(Description = "Reperimento dei filtri validi e applicabili alla migrazione dei documenti in DCTM")]
        public DocsPaDocumentale_PITRE.Migrazione.Documento.FiltroMigrazioneDocumento GetFiltroMigrazioneDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.Documento.FiltroMigrazioneDocumento filtro, out int numeroDocumenti)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Documento.GetFiltroMigrazione(amministrazione, filtro, out numeroDocumenti);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        [WebMethod()]
        public void InterrompiImportaDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Documento.Interrompi(amministrazione);
        }

        /// <summary>
        /// Migrazione in DCTM di tutti i documenti di un'amministrazioni PITRE
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        [WebMethod(Description = "Migrazione in DCTM di tutti i documenti di un'amministrazioni PITRE")]
        public void ImportaDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Documento.ImportaDocumenti(amministrazione, opzioniMigrazione);
        }

        /// <summary>
        /// Migrazione in DCTM di tutti i documenti relativi alle stampe registri di un'amministrazioni PITRE
        /// </summary>
        /// <param name="amministrazione"></param>
        [WebMethod(Description = "Migrazione in DCTM di tutti i documenti relativi alle stampe registri di un'amministrazioni PITRE")]
        public void ImportaDocumentiStampaRegistri(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            DocsPaDocumentale_PITRE.Migrazione.Documento.ImportaDocumentiStampaRegistri(amministrazione);
        }

        /// <summary>
        /// Verifica se i documenti siano stati modificati in PITRE e non in DCTM dopo essere stati migrati
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="documentiMigrazione"></param>
        /// <returns></returns>
        [WebMethod(Description = "Verifica se i documenti siano stati modificati in PITRE e non in DCTM dopo essere stati migrati")]
        public DocsPaDocumentale_PITRE.Migrazione.InfoDocumentoMigrazione[] VerificaDocumentiModificati(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.InfoDocumentoMigrazione[] documentiMigrazione)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Documento.VerificaDocumentiModificati(amministrazione, documentiMigrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="opzioniMigrazione"></param>
        /// <returns></returns>
        [WebMethod()]
        public DocsPaDocumentale_PITRE.Migrazione.InfoDocumentoMigrazione[] VerificaDocumenti(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.Documento.OpzioniMigrazioneDocumento opzioniMigrazione)
        {
            return DocsPaDocumentale_PITRE.Migrazione.Documento.VerificaDocumenti(amministrazione, opzioniMigrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="documenti"></param>
        [WebMethod(Description = "Migrazione in DCTM di tutti i documenti di un'amministrazioni PITRE")]
        public void ImportaDocumentiSelezionati(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, DocsPaDocumentale_PITRE.Migrazione.InfoDocumentoMigrazione[] documenti)
        {
            DocsPaDocumentale_PITRE.Migrazione.Documento.ImportaDocumentiSelezionati(amministrazione, documenti);
        }

    }
}