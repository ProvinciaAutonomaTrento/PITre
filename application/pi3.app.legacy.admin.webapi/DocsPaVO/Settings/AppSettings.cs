// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Collections.Generic;

namespace DocsPaVO.Settings
{
    public class AppSettings
    {
        public string LOG_LEVEL { get; set; }
        public string LOG_PATH { get; set; }
        public string DEBUG_PATH { get; set; }
        public string CHIAVE_TOKEN { get; set; }
        public string oracleSequenceName { get; set; }
        public string prefissiRubricaRiservati { get; set; }
        public string separator { get; set; }
        public string prefissoInsertCorr { get; set; }
        public string prefissoCorrOccasionale { get; set; }
        public string numeroMaxRisultatiQuery { get; set; }
        public string DOC_ROOT { get; set; }
        public string UPLOAD_BIGFILE_REPOSITORY { get; set; }
        public string PREVIEWS_PATH { get; set; }
        public string CHUNCK_PATH { get; set; }
        public string DOC_PATH { get; set; }
        public string importExportLogPath { get; set; }
        public string REPORTS_PATH { get; set; }
        public string XSDFilePath { get; set; }
        public string QueryFilePath { get; set; }
        public string ImportExportQuery { get; set; }
        public string ARCHIVIO_LOG_PATH { get; set; }
        public string EST_VIS_SUP_PARI_LIV { get; set; }
        public string DTD_SEGNATURA_PATH { get; set; }
        public string ELABORA_MAIL_ORDINARIE { get; set; }
        public string ELIMINA_MAIL_ELABORATE { get; set; }
        public string DBType { get; set; }
        public string MODELS_ROOT_PATH { get; set; }
        public string PKCS7InputDirectory { get; set; }
        public string PKCS7OutputDirectory { get; set; }
        public string RetrieveCRLTimeout { get; set; }
        public string CRLOnlineCheck { get; set; }
        public string pathAcquisizioneBatch { get; set; }
        public string UrlAcquisizioneBatch { get; set; }
        public string ELIMINA_RICEVUTE_PEC { get; set; }
        public string BATCH_INTEROP { get; set; }
        public string NUM_TENTATIVI_CONN_SVR_POSTA { get; set; }
        public string INTEROP_INT_NO_MAIL { get; set; }
        public string PDF_CONVERT_INLINE_ACTIVE { get; set; }
        public string INLINE_CONVERTER_URL { get; set; }
        public string PDF_CONVERTER_TYPE { get; set; }
        public string PDF_CONVERTIBLE_FILE_TYPES { get; set; }
        public string ProtoASLPathFolder { get; set; }
        public string FULLTEXT_INDEX_CATALOG { get; set; }
        public string FULLTEXT_INDEXING_WS { get; set; }
        public string SUPPORTED_FILE_TYPES_ENABLED { get; set; }
        public string FULLTEXT_SPECIAL_CHARACTERS { get; set; }
        public string FULLTEXT_WILDCARD_CHARACTER { get; set; }
        public string APPLICATION_NAME { get; set; }
        public string ENABLE_CANC_DOC_TRASMESSI { get; set; }
        public string ENABLE_RF { get; set; }
        public string POP3_CONN_TIMEOUT { get; set; }
        public string ARCHIVIO_LOG_PATH_AMM { get; set; }
        public string READ_IMAGES_ROOT_PATH { get; set; }
        public string IMAGES_ROOT_PATH { get; set; }
        public string ADMIN_IMAGES_ROOT_PATH { get; set; }
        public string MITT_DEST_ADDRESS { get; set; }
        public string IMAGES_PATH { get; set; }
        public string IS_ENABLED_PROFILAZIONE_ALLEGATI { get; set; }
        public string INDICE_SISTEMATICO { get; set; }
        public string INOLTRA_DOC { get; set; }
        public string LdapUserSyncActive { get; set; }
        public string TRASMISSIONE_AUTOMATICA { get; set; }
        public string documentale { get; set; }
        public string PitreDualFileWritingMode { get; set; }
        public string RubricaComuneAmministrazione { get; set; }
        public string RubricaComuneConnectionString { get; set; }
        public string CONVERSIONE_PDF_LATO_SERVER { get; set; }
        public string ADDRESS_SERVER_CODA_PDF { get; set; }
        public string PDF_CONVERT_SERVER_NOTIFY { get; set; }
        public string CONVERSIONE_PDF_SINCRONA_LC { get; set; }
        public string LIVECYCLE_SERVICE_SERVER { get; set; }
        public string LIVECYCLE_SERVICE_GENERATE_PDF_SERVICE { get; set; }
        public string LIVECYCLE_USERNAME { get; set; }
        public string LIVECYCLE_PASSWORD { get; set; }
        public string LIVECYCLE_SERVICE_PROCESS_FORM { get; set; }
        public string LIVECYCLE_SERVICE_PROCESS_BARCODE_FORM { get; set; }

        public string LIVECYCLE_SERVICE_GENERATE_PDF_SERVICE_SINCRONO { get; set; }
        public string LIVECYCLE_SERVICE_GENERATE_PDF_SINCRONO { get; set; }
        public string SET_DATA_VISTA_GRD { get; set; }
        public string INTEROP_SEGNATURA_SOLO_UO { get; set; }
        public string NON_EREDITA_MITTENTE_USCITA { get; set; }
        public string SESSION_REPOSITORY_DISABLED { get; set; }
        public string VISIB_POST_TRASM_WF { get; set; }
        public string DS_PROVIDER { get; set; }
        public string DS_EXTENDED_PROPERTIES { get; set; }
        public string FTP_ADDRESS { get; set; }
        public string FTP_USERNAME { get; set; }
        public string FTP_PASSWORD { get; set; }
        public string ENABLE_MITTENTI_MULTIPLI { get; set; }
        public string SEGNATURA_NEL_SUBJECT { get; set; }
        public string GESTIONE_RICEVUTE_PEC { get; set; }
        public string SALVA_RICEVUTE_PEC { get; set; }
        public string DTD_DATICERT_PATH { get; set; }
        public string APP_CHIAMANTE_TIMESTAMP { get; set; }
        public string ENABLE_TIMESTAMP { get; set; }
        public string TYPE_TSA { get; set; }
        public string URL_TSA { get; set; }
        public string CONSERVAZIONE_ROOT_PATH { get; set; }
        public string CONSERVAZIONE_DOWNLOAD_URL { get; set; }
        public string CONSERVAZIONE_README_PATH { get; set; }
        public string NumProtSup_86107 { get; set; }
        public string CONSERVAZIONE_GG_NOTIFICHE { get; set; }
        public string CLRVerificationModuleArgument { get; set; }

        public string MAIL_PROVIDER { get; set; }
        public string URL_PATH_IS { get; set; }
        public string HSMServiceUrl { get; set; }
        public string CONSERVAZIONE_REMOTE_STORAGE_URL { get; set; }
        public string ElasticApm_ServerUrl { get; set; }
        public string FATTURAZIONE_ELETTRONICA_EXTERNAL_SERVICE { get; set; }
        public string FATTURAZIONE_ELETTRONICA_WEB_REFERENCE_URL { get; set; }
        public string FTPSSL { get; set; }
        public string LdapIntegrationActive { get; set; }
        public string LdapUserSyncCertificatePath { get; set; }
        public string NumProtSup_11685 { get; set; }
        public string POP3_READ_TIMEOUT { get; set; }
        public string PUBLISHER_DISABLED { get; set; }
        public string PUBLISHER_STATIC_URL { get; set; }
        public string STATIC_ROOT_PATH { get; set; }

        // trovati nei sorgenti
        public string USE_CACHE { get; set; }
        public string DOC_TYPE { get; set; }
        public string ENABLE_PROTOCOLLO_TIT { get; set; }
        public string ENABLE_LIVELLI_TITOLARIO { get; set; }
        public string mittenteNotificaTrasmissione { get; set; }
        public string verifica_path_file { get; set; }
        public string PROTOCOLLAZIONE_LIBERA { get; set; }
        public string NO_FILTRO_AOO { get; set; }
        public string ENABLE_RIFERIMENTI_MITTENTE { get; set; }
        public string AS400 { get; set; }
        public string IMAP_CONN_TIMEOUT { get; set; }
        public string IMAP_READ_TIMEOUT { get; set; }
        public string DisableSelectTop { get; set; } = "0";
        public string sqlCommandTimeOut { get; set; } = null;
        public string ENABLE_CONTATORE_TIT { get; set; }

        public string CSS_ROOT_PATH { get; set; }

        public string WebRoot = "";
        
        public string LogRootPath = "";

        public static AppSettings Instance = new();
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}
