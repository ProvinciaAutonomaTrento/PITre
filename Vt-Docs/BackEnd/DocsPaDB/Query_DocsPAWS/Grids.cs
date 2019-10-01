using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using DocsPaVO.Grid;
using DocsPaVO.ProfilazioneDinamica;
using System.Xml.Serialization;
using DocsPaUtils;
using System.Data;
using System.IO;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{

    /// <summary>
    /// Questa classe fornisce funzionalità per la gestione delle impostazione
    /// relative ad una griglia
    /// </summary>
    public class Grids : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Grids));

        /// <summary>
        /// Nomi delle query per l'effettuazione delle operazioni CRUD sulle
        /// griglie
        /// </summary>
        private const string QueryNameForLoadSettings = "LOAD_GRID_SETTINGS";
        private const string QueryNameForSaveSettings = "SAVE_GRID_SETTINGS";
        private const string QueryNameForUpdateSettings = "UPDATE_GRID_SETTINGS";
        private const string QueryNameForStandardGrid = "GET_STANDARD_GRID_FOR_USER";
        private const string QueryNameForLoadGridFromSearchId = "GET_GRID_FROM_SEARCH_ID";
        private const string QueryNameForExistGridForSearchId = "EXIST_GRID_FOR_RAPID_SEARCH";
        private const string QueryNameForExistGridPersonalizationFunction = "EXIST_GRID_PERSONALIZATION_FUNCTION";
        private const string QueryNameForRetriveStandardGridId = "GET_STANDARD_GRID_ID";

        private const string QueryNameForRetriveDefinedGrids = "GET_DEFINED_GRIDS";
        private const string QueryNameForRemoveGrid = "REMOVE_GRID";
        private const string QueryNameForUserGrid = "GET_GRID_FOR_USER_CUSTUM";
        private const string QueryNameForRemovePreferredGrid = "REMOVE_PREFERRED_GRID";
        private const string QueryNameForInsertPreferred = "INSERT_DPA_ASS_GRIDS";
        private const string QueryNameForRemovePreferredGridById = "REMOVE_PREFERRED_GRID_BY_ID";
        private const string QueryNameForRemovePreferredGridByType = "REMOVE_PREFERRED_GRID_BY_TYPE";
        private const string QueryNameForUpdateSavedGrid = "UPDATE_GRID_BY_ID";
        private const string QueryNameForSaveSearchSettings = "SAVE_GRID_SEARCH_SETTINGS";
        private const string QueryNameForRetriveDefinedGridsOnlyRole = "GET_DEFINED_GRIDS_ONLY_ROLE";
        private const string QueryNameForRefreshGrid = "GET_ALL_GRIDS";

        /// <summary>
        /// Funzione per il caricamento delle informazioni relative ad una griglia.
        /// </summary>
        /// <param name="gridId">Identificativo univoco della griglia</param>
        /// <param name="gridType">Tipo di ricerca per cui è definita la griglia</param>
        /// <param name="userSystemId">Identificativo univoco dell'utente proprietario della personalizzazione</param>
        /// <param name="roleId">Identificativo univoco del ruolo. Introdotto per eventuali sviluppi futuri: caso di creazione di griglie standard associate al ruolo. In tal caso bisognerà cambiare le query.</param>
        /// <param name="administrationId">Identificativo univoco dell'amministrazione per cui è definita la griglia</param>
        /// <param name="templateId">Identificativo univoco del template di profilazione dinamica salvato nei criteri di ricerca relativi alla ricerca salvata</param>
        /// <returns>Le impostazioni relative alla griglia richiesta</returns>
        public Grid LoadGrid(
       String gridId,
       Grid.GridTypeEnumeration gridType,
       InfoUtente userInfo,
       List<String> templatesId)
        {
            /*
             * Criteri per la selezione delle informazioni sulla griglia.
             * 
             * Se è valorizzato gridId, viene utilizzato questo per caricare la griglia.
             * Se gridId non è valorizzato, viene controllato templateId. Se questo è valorizzato,
             * viene caricata la griglia standard per la ricerca basandosi su grigType. Verranno
             * caricate anche le informazioni sui campi relativi al templateId.
             * Se è null anche templateId, viene caricata la griglia standard definita per gridType.
             * 
             * Le griglia standard per una tipologia sono caratterizzate dall'avere userSystemId non
             * valorizzato.
             */

            // La griglia da restituire
            Grid toReturn;

            // Se gridId è valorizzato e diverso da -1 viene caricata la griglia associata
            if (!String.IsNullOrEmpty(gridId) && gridId != "-1")
                toReturn = this.LoadGrid(gridId);
            else
                // Altrimenti viene caricata la griglia standard per la ricerca in cui verrà
                // caricata la griglia insieme alle informazioni sull'eventuale template associato
                toReturn = this.LoadGrid(userInfo, templatesId, gridType);

            // Restituzione della griglia
            return toReturn;
        }

        /// <summary>
        /// Questa funzione carica la griglia standard per la tipologia di ricerca.
        /// </summary>
        /// <param name="administrationId">Identificativo univoco dell'amministrazione</param>
        /// <param name="templatesId">Identificsativo univoco dell'eventuale template di cui caricare i campi</param>
        /// <param name="roleId">Identificativo univoco del ruolo dell'utente per cui caricare la griglia</param>
        /// <param name="gridType">Tipo di ricerca in cui verrà inclusa la griglia</param>
        /// <returns>La griglia da visualizzare</returns>
        private Grid LoadGrid(InfoUtente userInfo, List<String> templatesId, Grid.GridTypeEnumeration gridType)
        {
            // La griglia da restituire
            Grid toReturn = new Grid("-1", templatesId, gridType);

            // Creazione di una griglia con i campi standard
            toReturn.Fields.AddRange(this.GetStandardGrid(gridType));

            // Se è valorizzato templateID, vengono caricati i campi standard per la tipologia
            if (templatesId != null)
                foreach (String templateId in templatesId)
                    toReturn.Fields.AddRange(
                        this.GetTemplateField(templateId, userInfo.idGruppo, userInfo.idAmministrazione, gridType));

            // Impostazione del filtro di ricerca e della direzione di ordinamento se non impostati
            if (toReturn.FieldForOrder == null)
            {
                toReturn.FieldForOrder = this.GetStandardFieldForOrder(gridType);
                toReturn.OrderDirection = Grid.OrderDirectionEnum.Desc;
            }

            // Impostazione del colore da associare alle celle quando sono vuote perché
            // associate ad un template non applicabile ad documento / fascicolo della riga
            toReturn.ColorForFieldWithotTemplate = "990000";

            // Restituzione della griglia
            return toReturn;

        }

        /// <summary>
        /// Questa funzione carica i campi standard per una data tipologia di ricerca
        /// </summary>
        /// <param name="gridType">Tipo di ricerca di cui caricare i campi standard</param>
        /// <returns>Lista dei campi standard</returns>
        private List<Field> GetStandardGrid(Grid.GridTypeEnumeration gridType)
        {
            // La lista da restituire
            List<Field> toReturn = new List<Field>();

            // Aggiunta del campo con i checkbox
            toReturn.Add(new SpecialField()
            {
                Locked = true,
                FieldType = SpecialFieldsEnum.CheckBox,
                Visible = true,
                Position = 0,
                OriginalLabel = "Caselle di selezione",
                Label = "Caselle di selezione",
                FieldId = "C1",
                Width = 20
            });

            // Aggiunta dei campi relativi alla tipologia di oggetto
            switch (gridType)
            {
                case Grid.GridTypeEnumeration.Document:
                case Grid.GridTypeEnumeration.DocumentInProject:
                    toReturn.AddRange(this.GetStandardFieldForDocument());
                    break;
                case Grid.GridTypeEnumeration.Project:
                    toReturn.AddRange(this.GetStandardFieldForProject());
                    break;
                case Grid.GridTypeEnumeration.Transmission:
                    toReturn.AddRange(this.GetStandardGridForTransmission());
                    break;

            }

            int i = toReturn.Count;
            // Aggiunta del campo con le informazioni sul template


            // Aggiunta del campo con le icone di ricerca
            toReturn.Add(new SpecialField()
            {
                Visible = true,
                Position = i,
                Locked = true,
                FieldType = SpecialFieldsEnum.Icons,
                OriginalLabel = "Icone",
                Label = "Icone",
                FieldId = "C2"
            });

            // Restituzione della lista di campi
            return toReturn;

        }

        /// <summary>
        /// Questa funzione restituisce la lista dei campi standard per la ricerca documenti
        /// </summary>
        /// <returns>Lista dei campi standard per la ricerca dei documenti</returns>
        private List<Field> GetStandardFieldForDocument()
        {
            // Lista da restituire
            List<Field> toReturn = new List<Field>();

            // Aggiunta dei campi standard
            toReturn.Add(this.GetFieldProperties(
                "Documento",
                "Doc",
                "D1",
                false,
                0,
                1,
                true,
                "A.DOCNUMBER",
                "A.DOCNUMBER",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Registro",
                "Registro",
                "D2",
                false,
                0,
                2,
                false,
                "getcodreg(a.id_registro)",
                "@dbuser@.getcodreg(a.id_registro)",
                "Standard",
                100));
            toReturn.Add(this.GetFieldProperties(
                "Tipo",
                "Tipo",
                "D3",
                false,
                0,
                3,
                true,
                "UPPER(TRIM(A.CHA_TIPO_PROTO))",
                "UPPER(LTRIM(RTRIM(A.CHA_TIPO_PROTO)))",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Oggetto",
                "Oggetto",
                "D4",
                false,
                0,
                4,
                true,
                "UPPER(TRIM(A.VAR_PROF_OGGETTO))",
                "UPPER(LTRIM(RTRIM(A.VAR_PROF_OGGETTO)))",
                "Standard",
                200));
            toReturn.Add(this.GetFieldProperties(
                "Mittente / Destinatario",
                "Mitt/Dest",
                "D5",
                false,
                0,
                5,
                true,
                "corrcat(a.system_id, a.cha_tipo_proto)",
                "@dbuser@.corrcat(a.system_id, a.cha_tipo_proto)",
                "Standard",
                200));
            toReturn.Add(this.GetFieldProperties(
                "Mittente",
                "Mittente",
                "D6",
                false,
                0,
                6,
                false,
                "corrcatbytipo(a.docnumber, a.cha_tipo_proto, 'M')",
                "@dbuser@.corrcatbytipo(a.docnumber, a.cha_tipo_proto, 'M')",
                "Standard",
                100));
            toReturn.Add(this.GetFieldProperties(
                "Destinatari",
                "Destinatari",
                "D7",
                false,
                0,
                7,
                false,
                "corrcatbytipo(a.docnumber, a.cha_tipo_proto, 'D')",
                "@dbuser@.corrcatbytipo(a.docnumber, a.cha_tipo_proto, 'D')",
                "Standard",
                100));
            toReturn.Add(this.GetFieldProperties(
                "Segnatura",
                "Segnatura (DOC)",
                "D8",
                false,
                0,
                8,
                false,
                "UPPER(TRIM(A.VAR_SEGNATURA))",
                "UPPER(LTRIM(RTRIM(A.VAR_SEGNATURA)))",
                "Standard",
                100));
            toReturn.Add(this.GetFieldProperties(
                "Data protocollazione / Creazione",
                "Data protocollazione / Creazione",
                "D9",
                false,
                0,
                9,
                false,
                "NVL (a.dta_proto, a.creation_time)",
                "ISNULL (a.dta_proto, a.creation_time)",
                "Standard",
                100));
            toReturn.Add(this.GetFieldProperties(
                "Esito pubblicazione",
                "Esito pubblicazione",
                "D10",
                false,
                0,
                10,
                false,
                "getEsitoPubblicazione(a.system_id)",
                "@dbuser@.getEsitoPubblicazione(a.system_id)",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Data annullamento",
                "Data annullamento",
                "D11",
                false,
                0,
                11,
                false,
                "A.DTA_ANNULLA",
                "A.DTA_ANNULLA",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Numero protocollo",
                "Num. Prot.",
                "D12",
                false,
                0,
                12,
                false,
                "A.NUM_PROTO",
                "A.NUM_PROTO",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Codice autore",
                "Codice autore",
                "D13",
                false,
                0,
                13,
                false,
                "UPPER(trim(getpeopleuserid(a.author)))",
                "UPPER(LTRIM(RTRIM(@dbuser@.getPeopleUserId(A.AUTHOR))))",
                "Standard",
                100));
            toReturn.Add(this.GetFieldProperties(
                "Data archiviazione",
                "Data archiviazione",
                "D14",
                false,
                0,
                14,
                false,
                "a.archive_date",
                "a.archive_date",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Personale",
                "Personale",
                "D15",
                false,
                0,
                15,
                false,
                "a.cha_personale",
                "a.cha_personale",
                "Standard",
                50));
            toReturn.Add(this.GetFieldProperties(
                "Privato",
                "Privato",
                "D16",
                false,
                0,
                16,
                false,
                "a.cha_privato",
                "a.cha_privato",
                "Standard",
                50));
            if (IsPresentsNote())
            {
                toReturn.Add(this.GetFieldProperties(
                "Note",
                "Note",
                "D17",
                false,
                0,
                17,
                false,
                "esisteNotaVisibile ('D',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                "@dbuser@.esisteNotaVisibile ('D',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                "Standard",
                100));
            }
            else
            {
                toReturn.Add(this.GetFieldProperties(
                "Note",
                "Note",
                "D17",
                false,
                0,
                17,
                false,
                "gettestoultimanota ('D',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                "@dbuser@.gettestoultimanota ('D',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                "Standard",
                100));
            }

            toReturn.Add(this.GetFieldProperties(
           "Cod. Fascicoli",
           "Cod. Fascicoli",
           "D18",
           false,
           0,
           18,
           false,
           "classcat(a.system_id)",
           "@dbuser@.classcat(a.system_id)",
           "Standard",
           100));

            toReturn.Add(this.GetFieldProperties(
          "Nome e cognome autore",
          "Nome e cognome autore",
          "D19",
          false,
          0,
          19,
          false,
          "getpeoplename (a.author)",
          "@dbuser@.getpeoplename (a.author)",
          "Standard",
          100));

            toReturn.Add(this.GetFieldProperties(
            "Ruolo autore",
            "Ruolo autore",
            "D20",
            false,
            0,
            20,
            false,
            "getdesccorr(a.id_ruolo_creatore)",
            "@dbuser@.getdesccorr(a.id_ruolo_creatore)",
            "Standard",
            100));

            toReturn.Add(this.GetFieldProperties(
            "Data arrivo",
            "Data arrivo",
            "D21",
            false,
            0,
            21,
            false,
            "getdataarrivodoc(a.docnumber)",
            "@dbuser@.getdataarrivodoc(a.docnumber)",
            "Standard",
            50));

            toReturn.Add(this.GetFieldProperties(
            "Stato del documento",
            "Stato del documento",
            "D22",
            false,
            0,
            22,
            false,
            "getdiagrammistato(a.docnumber, 'D')",
            "@dbuser@.getdiagrammistato(a.docnumber, 'D')",
            "Standard",
            100));

            toReturn.Add(this.GetFieldProperties(
            "File",
            "File",
            "D23",
            false,
            0,
            23,
            false,
            "getchaimg (a.docnumber)",
            "@dbuser@.getchaimg (a.docnumber)",
            "Standard",
            100));

            string valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
            {
                toReturn.Add(this.GetFieldProperties(
                    "Atipicita",
                    "Atipicità",
                    "D24",
                    false,
                    0,
                    24,
                    false,
                    "cha_cod_t_a",
                    "cha_cod_t_a",
                    "Standard",
                    100));
            }

            toReturn.Add(this.GetFieldProperties(
             "Tipologia",
             "Tipologia",
             "U1",
             false,
             0,
             24,
             false,
             "ta.var_desc_atto",
             "ta.var_desc_atto",
             "Standard",
             100));

            toReturn.Add(this.GetFieldProperties(
         "Impronta",
         "Impronta",
         "IMPRONTA",
         false,
         0,
         25,
         false,
         "getImpronta(a.docnumber)",
         "@dbuser@.getImpronta(a.docnumber)",
         "Standard",
         100));

            toReturn.Add(this.GetFieldProperties(
          "Codice applicazione",
          "Codice applicazione",
          "COD_EXT_APP",
          false,
          0,
          19,
          false,
          "COD_EXT_APP",
          "COD_EXT_APP",
          "",
          100));

            toReturn.Add(this.GetFieldProperties(
            "Nome Originale",
            "Nome Originale",
            "NOME_ORIGINALE",
            false,
            0,
            26,
            false,
            "getNomeOriginale(a.docnumber)",
            "@dbuser@.getNomeOriginale(a.docnumber)",
            "Standard",
            100));

            toReturn.Add(this.GetFieldProperties(
        "Data AdL",
        "Data AdL",
        "DTA_ADL",
        false,
        0,
        27,
        false,
        "GetDateInADL(a.docnumber, 'D', @idGruppo@, @idPeople@)",
        "@dbuser@.GetDateInADL (a.system_id, 'D', @idGruppo@, @idPeople@)",
        "Standard",
        50));
            // PEC 4 Requisito 3: ricerca documenti spediti
            toReturn.Add(this.GetFieldProperties(
                "Esito Spedizione",
                "Esito Spedizione",
                "esito_spedizione",
                false, 0,
                28,
                false,
                "getEsitoSpedizione(a.system_id)",
                "@dbuser@.getEsitoSpedizione(a.system_id)", // da implementare per SQL
                "Standard",
                200));
            // PEC 4 Requisito 3: ricerca documenti spediti
            toReturn.Add(this.GetFieldProperties(
                "Num. Ricevute",
                "Num. Ricevute",
                "count_ric_interop",
                false, 0,
                29,
                false,
                "getCountRicevuteInterop(a.system_id, @tipoRicevutaInteroperante@)",
                "@dbuser@.getCountRicevuteInterop(a.system_id, @tipoRicevutaInteroperante@)", //da implementare per SQLServer
                "Standard",
                100));
            // INTEGRAZIONE PITRE-PARER - stato conservazione documento
            //string tipoConservazione = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
                toReturn.Add(this.GetFieldProperties(
                    "Stato Conservazione",
                    "Stato Conservazione",
                    "stato_conservazione",
                    false, 0,
                    30,
                    false,
                    "StatoConservazione",
                    "StatoConservazione",
                    "Standard",
                    100));

                //// conservazione interna
                //toReturn.Add(this.GetFieldProperties(
                //    "Stato conservazione",
                //    "Stato conservazione",
                //    "is_doc_conservato", false, 0,
                //    30,
                //    false,
                //    "getInConservazioneDoc(A.SYSTEM_ID)",
                //    "@dbuser@.getInConservazioneDoc(A.SYSTEM_ID)",
                //    "Standard",
                //    100
                //    ));

            // INTEGRAZIONE PITRE-PARER - MEV Policy - codice policy
            toReturn.Add(this.GetFieldProperties(
                "Codice Policy",
                "Codice Policy",
                "CODICE_POLICY", false, 0,
                31,
                false,
                "getPolicyVersamentoCod(A.SYSTEM_ID)",
                "@dbuser@.getPolicyVersamentoCod(A.SYSTEM_ID)",
                "Standard",
                100
                ));
            // contatore esecuzioni policy
            toReturn.Add(this.GetFieldProperties(
                "Num. esecuzione policy",
                "Num. esecuzione policy",
                "CONTATORE_POLICY", false, 0,
                32,
                false,
                "getPolicyVersamentoCounter(A.SYSTEM_ID)",
                "@dbuser@.getPolicyVersamentoCounter(A.SYSTEM_ID)",
                "Standard",
                50
                ));
            // data esecuzione policy
            toReturn.Add(this.GetFieldProperties(
                "Data esecuzione policy",
                "Data esecuzione policy",
                "DATA_ESECUZIONE_POLICY", false, 0,
                33,
                false,
                "getPolicyVersamentoDataExec(A.SYSTEM_ID)",
                "@dbuser@.getPolicyVersamentoDataExec(A.SYSTEM_ID)",
                "Standard",
                100
                ));
            // task status
            toReturn.Add(this.GetFieldProperties(
                "Stato attivita",
                "Stato attività",
                "CHA_TASK_STATUS", false, 0,
                34,
                false,
                "CHA_TASK_STATUS",
                "CHA_TASK_STATUS",
                "Standard",
                100
                ));

            toReturn.Add(this.GetFieldProperties(
                "Motivo AdL",
                "Motivo AdL",
                "MOTIVO_ADL",
                false,
                0,
                35,
                false,
                "GetMotivoADL(a.docnumber, 'D', @idGruppo@, @idPeople@)",
                "@dbuser@.GetMotivoADL (a.system_id, 'D', @idGruppo@, @idPeople@)",
                "Standard",
                100));
            // Restituzione della lista
            return toReturn;

        }

        /// <summary>
        /// Questa funzione restituisce i campi standard per la ricerca fascicoli
        /// </summary>
        /// <returns>Lista dei campi standard per la ricerca fascicoli</returns>
        private List<Field> GetStandardFieldForProject()
        {
            // Lista da restituire
            List<Field> toReturn = new List<Field>();

            // Aggiunta dei campi standard
            toReturn.Add(this.GetFieldProperties(
                "Tipo",
                "Tipo",
                "P1",
                false,
                0,
                1,
                false,
                "A.cha_tipo_fascicolo",
                "A.cha_tipo_fascicolo",
                "Standard",
                50));

            toReturn.Add(this.GetFieldProperties(
                "Cod Class",
                "Cod Class",
                "P2",
                false,
                0,
                2,
                true,
                "getcodtit(a.id_parent)",
                "@dbuser@.getcodtit(a.id_parent)",
                "Standard",
                50));

            toReturn.Add(this.GetFieldProperties(
               "Codice",
               "Codice",
               "P3",
               false,
               0,
               3,
               true,
               "A.VAR_CODICE",
               "A.VAR_CODICE",
               "Standard",
               50));

            toReturn.Add(this.GetFieldProperties(
              "Descrizione",
              "Descrizione",
              "P4",
              false,
              0,
              4,
              true,
              "UPPER(TRIM(A.DESCRIPTION))",
              "UPPER(LTRIM(RTRIM(A.DESCRIPTION)))",
              "Standard",
              100));

            toReturn.Add(this.GetFieldProperties(
                "Apertura",
                "Apertura",
                "P5",
                false,
                0,
                5,
                true,
                "A.DTA_APERTURA",
                "A.DTA_APERTURA",
                "Standard",
                100));

            toReturn.Add(this.GetFieldProperties(
                "Chiusura",
                "Chiusura",
                "P6",
                false,
                0,
                6,
                true,
                "A.DTA_CHIUSURA",
                "A.DTA_CHIUSURA",
                "Standard",
                100));

            toReturn.Add(this.GetFieldProperties(
                 "AOO",
                 "AOO",
                 "P7",
                 false,
                 0,
                 7,
                 false,
                 "getcodreg(a.id_registro)",
                 "@dbuser@.getcodreg(a.id_registro)",
                 "Standard",
                 50));

            if (IsPresentsNote())
            {
                toReturn.Add(this.GetFieldProperties(
                   "Note",
                   "Note",
                   "P8",
                   false,
                   0,
                   8,
                   false,
                   "esisteNotaVisibile ('F',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                   "@dbuser@.esisteNotaVisibile ('F',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                   "Standard",
                   200));
            }
            else
            {
                toReturn.Add(this.GetFieldProperties(
                    "Note",
                    "Note",
                    "P8",
                    false,
                    0,
                    8,
                    false,
                    "gettestoultimanota ('F',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                    "@dbuser@.gettestoultimanota ('F',a.system_id, @idGruppo@,@idPeople@,@idGruppo@)",
                    "Standard",
                    200));
            }

            toReturn.Add(this.GetFieldProperties(
                "Privato",
                "Privato",
                "P9",
                false,
                0,
                9,
                false,
                "a.cha_privato",
                "a.cha_privato",
                "Standard",
                50));

            toReturn.Add(this.GetFieldProperties(
                "Titolario",
                "Titolario",
                "P10",
                false,
                0,
                10,
                false,
                "getDescTitolario(a.id_titolario)",
                "@dbuser@.getDescTitolario(a.id_titolario)",
                "Standard",
                100));

            toReturn.Add(this.GetFieldProperties(
                "Cartaceo",
                "Cartaceo",
                "P11",
                false,
                0,
                11,
                false,
                "A.CARTACEO",
                "A.CARTACEO",
                "Standard",
                50));

            toReturn.Add(this.GetFieldProperties(
               "In archivio",
               "In archivio",
               "P12",
               false,
               0,
               12,
               false,
               "a.cha_in_archivio",
               "a.cha_in_archivio",
               "Standard",
               50));

            toReturn.Add(this.GetFieldProperties(
             "In conservazione",
             "In conservazione",
             "P13",
             false,
             0,
             13,
             false,
             "getinconservazione (NULL,a.system_id,'F',@idPeople@,@idGruppo@)",
             "@dbuser@.getinconservazione (NULL,a.system_id,'F',@idPeople@,@idGruppo@)",
             "Standard",
             50));

            toReturn.Add(this.GetFieldProperties(
            "Num fascicolo",
            "Num fascicolo",
            "P14",
            false,
            0,
            14,
            false,
            "to_number(a.num_fascicolo)",
            "convert(int, a.num_fascicolo)",
            "Standard",
            50));

            toReturn.Add(this.GetFieldProperties(
            "Num mesi in conservazione",
            "Num mesi in conservazione",
            "P15",
            false,
            0,
            15,
            false,
            "to_number(a.NUM_MESI_CONSERVAZIONE)",
            "convert(int, a.NUM_MESI_CONSERVAZIONE)",
            "Standard",
            50));

            toReturn.Add(this.GetFieldProperties(
           "Stato",
           "Stato",
           "P16",
           false,
           0,
           16,
           false,
           "getdiagrammistato (a.system_id, 'F')",
           "@dbuser@.getdiagrammistato (a.system_id, 'F')",
           "Standard",
           50));

            toReturn.Add(this.GetFieldProperties(
        "Nome e cognome autore",
        "Nome e cognome autore",
        "P17",
        false,
        0,
        17,
        false,
        "getpeoplename (a.author)",
        "@dbuser@.getpeoplename (a.author)",
        "Standard",
        50));

            toReturn.Add(this.GetFieldProperties(
                 "Ruolo autore",
                 "Ruolo autore",
                 "P18",
                 false,
                 0,
                 18,
                 false,
                 "getdesccorr(a.id_ruolo_creatore)",
                 "@dbuser@.getdesccorr(a.id_ruolo_creatore)",
                 "Standard",
                 100));

            toReturn.Add(this.GetFieldProperties(
            "Uo creatore",
            "Uo creatore",
            "P19",
            false,
            0,
            19,
            false,
            "getdesccorr(a.id_uo_creatore)",
            "@dbuser@.getdesccorr(a.id_uo_creatore)",
            "Standard",
            100));

            toReturn.Add(this.GetFieldProperties(
            "Data creazione",
            "Data creazione",
            "P20",
            false,
            0,
            20,
            false,
            "A.DTA_CREAZIONE",
             "A.DTA_CREAZIONE",
            "Standard",
            50));

            toReturn.Add(this.GetFieldProperties(
            "Tipologia",
            "Tipologia",
            "U1",
            false,
            0,
            21,
            false,
            "ta.var_desc_fasc",
            "ta.var_desc_fasc",
            "Standard",
            100));

            toReturn.Add(this.GetFieldProperties(
            "Collocazione fisica",
            "Collocazione fisica",
            "P22",
            false,
            0,
            22,
            false,
            "getdesccorr(a.id_uo_lf)",
            "@dbuser@.getdesccorr(a.id_uo_lf)",
            "Standard",
            100));


            toReturn.Add(this.GetFieldProperties(
  "Data AdL",
  "Data AdL",
  "DTA_ADL",
  false,
  0,
  23,
  false,
  "GetDateInADL(a.system_id, 'F', @idGruppo@, @idPeople@)",
  "@dbuser@.GetDateInADL (a.system_id, 'D', @idGruppo@, @idPeople@)",
  "Standard",
  50));

            string valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
            {
                toReturn.Add(this.GetFieldProperties(
                    "Atipicita",
                    "Atipicità",
                    "P23",
                    false,
                    0,
                    24,
                    false,
                    "cha_cod_t_a",
                    "cha_cod_t_a",
                    "Standard",
                    100));
            }

            toReturn.Add(this.GetFieldProperties(
            "Motivo AdL",
            "Motivo AdL",
            "MOTIVO_ADL",
            false,
            0,
            25,
            false,
            "GetMotivoADL(a.system_id, 'F', @idGruppo@, @idPeople@)",
            "@dbuser@.GetMotivoADL (a.system_id, 'D', @idGruppo@, @idPeople@)",
            "Standard",
            100));

            // Restituzione della lista
            return toReturn;
        }

        /// <summary>
        /// Questa funzione restituisce i campi standard per la ricerca fascicoli
        /// </summary>
        /// <returns>Lista dei campi standard per la ricerca fascicoli</returns>
        private List<Field> GetStandardGridForTransmission()
        {
            // Lista da restituire
            List<Field> toReturn = new List<Field>();

            toReturn.Add(this.GetFieldProperties(
                "Ragione di trasmissione",
                "Ragione di trasmissione",
                "T7",
                false,
                0,
                1,
                true,
                "UPPER(TRIM(GetTransReasonDesc(B.ID_RAGIONE)))",
                "UPPER(LTRIM(RTRIM(@dbUser@.GetTransReasonDesc(B.ID_RAGIONE))))",
                "Standard",
                200));
            // Restituzione della lista
            return toReturn;
        }

        /// <summary>
        /// Questa funzione restituisce la lista dei campi profilati relativi ad un template
        /// </summary>
        /// <param name="templateId">Identificativo del template da caricare</param>
        /// <param name="roleId">Identificativo del ruolo</param>
        /// <param name="administrationId">Identificativo dell'amministrazione</param>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Lista dei campi relativi al template</returns>
        private List<Field> GetTemplateField(String templateId, string roleId, string administrationId, Grid.GridTypeEnumeration gridType)
        {
            // Lista da restituire
            List<Field> toReturn;

            // Il template di ricerca
            Templates template = null;

            // Lista delle informazioni di visibilità sui campi
            AssDocFascRuoli[] visibilityInformation = null;

            // Inizializzazione della lista dei campi da restituire
            toReturn = new List<Field>();

            switch (gridType)
            {
                case Grid.GridTypeEnumeration.Document:
                case Grid.GridTypeEnumeration.DocumentInProject:
                    // Caricamento del template
                    template = new Model().getTemplateById(templateId);
                    // Caricamento delle informazioni di visiilità sui campi del template
                    visibilityInformation = (AssDocFascRuoli[])(
                        new Model().getDirittiCampiTipologiaDoc(
                            roleId, templateId).ToArray(typeof(AssDocFascRuoli)));

                    break;
                case Grid.GridTypeEnumeration.Project:
                    // Caricamento del template
                    template = new ModelFasc().getTemplateFascById(templateId);

                    // Caricamento delle informazioni di visibilità sui campi del template
                    visibilityInformation = (AssDocFascRuoli[])(new ModelFasc().getDirittiCampiTipologiaFasc(
                        roleId, templateId).ToArray(typeof(AssDocFascRuoli)));

                    break;
            }

            // Filtraggio delle visibilità in modo da selezionare solo le informazioni
            // con VIS_OGG_CUSTOM = "1"
            if (visibilityInformation != null)
                visibilityInformation = visibilityInformation.Where(e => e.VIS_OGG_CUSTOM == "1").ToArray();

            int nextId = toReturn.Count;
            OggettoCustom[] customObjects;
            // Creazione dei campi
            if (template != null)
            {
                customObjects = (OggettoCustom[])(template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom)));
                foreach (OggettoCustom obj in customObjects)
                    if (visibilityInformation.Where(e => e.ID_OGGETTO_CUSTOM == obj.SYSTEM_ID.ToString()).Count() > 0)
                        toReturn.Add(this.GetFieldProperties(
                            obj.DESCRIZIONE,
                            obj.DESCRIZIONE,
                            obj.SYSTEM_ID.ToString(),
                            obj.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CASELLADISELEZIONE"),
                            obj.SYSTEM_ID,
                            nextId++,
                            true,
                            obj.SYSTEM_ID.ToString(),
                            obj.SYSTEM_ID.ToString(),
                            template.DESCRIZIONE,
                            100));
            }
            // Restituzione della lista dei campi
            return toReturn;

        }

        /// <summary>
        /// Questa funzione crea un oiggetto con informazioni su un campo con un certo nome
        /// </summary>
        /// <param name="fieldName">Il nome da assegnare al campo</param>
        /// <param name="fieldLabel">Etichetta del campo</param>
        /// <param name="fieldType">Tipo di campo</param>
        /// <param name="id">Id da assegnare al campo</param>
        /// <param name="canAssumeMultipleValue">True se il campo è un multivalore</param>
        /// <param name="customObjectId">Identificativo univoco dell'oggetto custom associato al campo</param>
        /// <param name="position">Posizione del campo all'interno della griglia</param>
        /// <param name="visible">True se il campo deve essere visualizzato</param>
        /// <param name="dbOracleColumnName">Nome della colonna del DB Oracle con cui mappare il campo da creare oppure system id nel caso in cui il campo sia un campo profilato</param>
        /// <param name="dbSQLColumnName">Nome della colonna del DB SQL con cui mappare il campo da creare oppure system id nel caso in cui il campo sia un campo profilato</param>
        /// <param name="isCounterField">True se il campo custom è un contatore</param>
        /// <returns>Il campo creato</returns>
        private Field GetFieldProperties(string fieldName, string fieldLabel, String id, bool canAssumeMultipleValue, int customObjectId, int position, bool visible, String dbOracleColumnName, String dbSQLColumnName, String templateName, int fieldWidth)
        {
            // Il campo da restituire
            Field toReturn = new Field()
            {
                FieldId = id,
                Label = fieldLabel,
                OriginalLabel = fieldName,
                Visible = visible,
                CanAssumeMultiValues = canAssumeMultipleValue,
                CustomObjectId = customObjectId,
                AssociatedTemplateName = templateName,
                Position = position,
                //MaxLength = 100,
                MaxLength = -1,
                Width = fieldWidth,
                OracleDbColumnName = dbOracleColumnName,
                SqlServerDbColumnName = dbSQLColumnName
            };

            // Restituzione del campo
            return toReturn;

        }

        /// <summary>
        /// Questa funzione restituisce la griglia di emergenza. Viene richiamata nel caso in cui non sia stato
        /// possibile caricare una griglia e restituisce una griglia standard
        /// </summary>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia di emergenza</returns>
        public Grid GetEmergencyGrid(Grid.GridTypeEnumeration gridType)
        {
            // La griglia da restituire
            Grid toReturn = new Grid(String.Empty, null, gridType);

            // Aggiunta dei campi standard
            toReturn.Fields = this.GetStandardGrid(gridType);

            // Ripristino del filtro di ordinamento
            toReturn.FieldForOrder = this.GetStandardFieldForOrder(gridType);
            toReturn.OrderDirection = Grid.OrderDirectionEnum.Desc;

            // Restituzione della griglia creata
            return toReturn;

        }

        /// <summary>
        /// Questa funzione salva i dati relativi ad una griglia
        /// Se grid id è valorizzato, viene effettuato un aggiornamento dei dati relativi alla griglia
        /// altrimenti i dati vengono direttamente inseriti
        /// </summary>
        /// <param name="grid">La griglia da salvare</param>
        /// <param name="userSystemId">Identificativo dell'utente proprietario della griglia</param>
        /// <param name="roleId">Identificativo della griglia</param>
        /// <param name="administrationId">Identificativo del ruolo cui appartiene l'utente proprietario della griglia</param>
        /// <returns>Id della griglia creata / modificata</returns>
        public String SaveGrid(
             Grid grid,
             String userSystemId,
             String roleId,
             String administrationId,
             String gridName,
             Boolean isTemporary,
             String isActive, String visibility)
        {
            // Id della griglia creata / modificata
            String gridId;

            bool existGridForRapidSearch = true;

            if (!string.IsNullOrEmpty(visibility))
            {
                if (visibility.Equals("user"))
                {
                    visibility = "U";
                }
                else
                {
                    visibility = "R";
                }
            }

            if (!String.IsNullOrEmpty(grid.RapidSearchId) &&
                    grid.RapidSearchId != "-1")
                existGridForRapidSearch = this.ExistSavedGridForSavedSearch(grid.RapidSearchId, userSystemId, roleId, administrationId);

            if (String.IsNullOrEmpty(grid.GridId))
            {
                if (isActive.Equals("Y"))
                {
                    //    this.UpdatePreferred(grid, userSystemId, roleId, administrationId);
                }
                gridId = this.InsertNewGrid(grid, userSystemId, roleId, administrationId, gridName, isTemporary, isActive, visibility);
            }
            else
                if (existGridForRapidSearch && !grid.GridId.Equals("-2"))
                {
                    if (isActive.Equals("Y"))
                    {
                        //    this.UpdatePreferred(grid, userSystemId, roleId, administrationId);
                    }
                    gridId = this.UpdateGrid(grid, isActive, gridName, visibility);
                }
                else
                {
                    if (isActive.Equals("Y"))
                    {
                        //      this.UpdatePreferred(grid, userSystemId, roleId, administrationId);
                    }
                    gridId = this.InsertNewGrid(grid, userSystemId, roleId, administrationId, gridName, isTemporary, isActive, visibility);
                }


            // Restituzione dell'id della griglia
            return gridId;

        }

        /// <summary>
        /// Questa funzione carica le informazioni su una specifica griglia.
        /// </summary>
        /// <param name="gridId">Identificativo univoco della griglia da caricare</param>
        /// <param name="administrationId">L'id dell'amministrazione cui appartiene l'utente</param>
        /// <param name="templateId">Identificativo univoco dell'eventuale template di cui caricare i campi visibili al ruolo</param>
        /// <param name="roleId">Identificativo del rolo dell'utente per cui caricare la griglia</param>
        /// <param name="gridType">Tipo di ricerca in cui andrà inserita la griglia</param>
        /// <returns>La griglia richiesta</returns>
        private Grid LoadGrid(String gridId)
        {
            // Griglia restituita
            Grid toReturn = null;

            // Reperimento delle informaizoni sulla griglia dal DB
            Query query = InitQuery.getInstance().getQuery(QueryNameForLoadSettings);
            query.setParam("gridId", gridId);

            // Esecuzione della query
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - LoadGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - LoadGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                DataSet resultDataSet = new DataSet();
                dbProvider.ExecuteQuery(resultDataSet, commandText);

                if (resultDataSet.Tables[0].Rows.Count != 0)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(resultDataSet.Tables[0].Rows[0]["SERIALIZED_GRID"].ToString()));
                    toReturn = (Grid)deserializer.Deserialize(stream);
                }

            }

            // Restituzione della griglia
            return toReturn;
        }

        /// <summary>
        /// Funzione per il salvataggio dei dati relativi ad una nuova griglia
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userSystemId">Identificativo dell'utente proprietario della griglia</param>
        /// <param name="roleId">Identificativo del ruolo cui apprtiene l'utente proprietario della griglia</param>
        /// <param name="administrationId">Identificativo dell'amministrazione cui appartiene l'utente</param>
        /// <returns>System id assegnato alla griglia</returns>
        private String InsertNewGrid(Grid grid, string userSystemId, string roleId, string administrationId, String gridName, Boolean isTemporary, String isActive, String visibility)
        {
            // System id della griglia creata
            String gridId;

            // Stringa con il risultato della serializzazione della griglia
            String serializedGrid;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                // Salvataggio delle informaizoni sulla griglia sul DB, se la griglia serializzata è valorizzata
                Query query = InitQuery.getInstance().getQuery(QueryNameForSaveSettings);
                query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_GRIDS"));
                query.setParam("userId", userSystemId);
                query.setParam("roleId", roleId);
                query.setParam("administrationId", administrationId);
                query.setParam("searchId", String.IsNullOrEmpty(grid.RapidSearchId) ? "-1" : grid.RapidSearchId);
                query.setParam("typeGrid", grid.GridType.ToString());
                query.setParam("gridName", gridName);
                query.setParam("isTemporary", isTemporary ? "Y" : "N");
                query.setParam("isActive", isActive.ToString());
                query.setParam("visibility", visibility);

                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);

                // Recupero id e inserimento campo CLOB
                string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_GRIDS");
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + sql);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + sql);
                dbProvider.ExecuteScalar(out gridId, sql);

                if (!string.IsNullOrEmpty(gridId))
                {
                    grid.GridId = gridId;

                    // File XML serializzato da salvare sul db
                    using (MemoryStream stream = new MemoryStream())
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                        mySerializer.Serialize(stream, grid);
                        stream.Position = 0;
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        serializedGrid = Convert.ToBase64String(data);
                    }

                    dbProvider.SetLargeText("DPA_GRIDS", gridId, "SERIALIZED_GRID", serializedGrid);
                }
            }

            return gridId;

        }

        /// <summary>
        /// Funzione per l'aggiornamento dei dati relativi ad una griglia
        /// </summary>
        /// <param name="grid">Griglia da aggiornare</param>
        /// <param name="userSystemId">Identificativo dell'utente proprietario della griglia</param>
        /// <param name="roleId">Identificativo del ruolo</param>
        /// <param name="administrationId">Identificativo dell'amministrazione</param>
        /// <returns>Id della griglia modificata</returns>
        private String UpdateGrid(Grid grid, String isActive, String gridName, String visibility)
        {
            // Stringa con il risultato della serializzazione della griglia
            String serializedGrid;

            // Aggiornamento di nome e stato di attivazione della griglia
            using (DBProvider dataProvider = new DBProvider())
            {
                //Query query = new Query(QueryNameForUpdateSettings);
                Query query = InitQuery.getInstance().getQuery(QueryNameForUpdateSettings);
                query.setParam("isActive", isActive.ToString());
                query.setParam("gridName", gridName.ToString());
                query.setParam("gridId", grid.GridId);
                if (string.IsNullOrEmpty(visibility))
                {
                    //INSERIRE
                    //        query.setParam("visibility", grid.);
                }
                else
                {
                    query.setParam("visibility", visibility);
                }

                dataProvider.ExecuteNonQuery(query.getSQL());
            }

            // File XML serializzato da salvare sul db
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                mySerializer.Serialize(stream, grid);
                stream.Position = 0;
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                serializedGrid = Convert.ToBase64String(data);
            }

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.SetLargeText("DPA_GRIDS", grid.GridId, "SERIALIZED_GRID", serializedGrid);
            }

            return grid.GridId;

        }

        public Grid GetStandardGridForUser(InfoUtente userInfo, Grid.GridTypeEnumeration gridType)
        {
            // Griglia restituita
            Grid toReturn = null;

            //Selezione della griglia standard per l'utente
            Query query = InitQuery.getInstance().getQuery(QueryNameForStandardGrid);
            query.setParam("userId", userInfo.idPeople);
            query.setParam("roleId", userInfo.idGruppo);
            query.setParam("administrationId", userInfo.idAmministrazione);
            query.setParam("typeGrid", gridType.ToString());

            // Esecuzione della query
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetStandardGridForUser - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetStandardGridForUser - DocsPaDB/Grids.cs - QUERY : " + commandText);
                DataSet resultDataSet = new DataSet();
                dbProvider.ExecuteQuery(resultDataSet, commandText);

                if (resultDataSet.Tables[0].Rows.Count != 0)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(resultDataSet.Tables[0].Rows[0]["SERIALIZED_GRID"].ToString()));
                    toReturn = (Grid)deserializer.Deserialize(stream);
                    toReturn.GridId = resultDataSet.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    toReturn.RapidSearchId = resultDataSet.Tables[0].Rows[0]["SEARCH_ID"].ToString();
                }
                else
                    toReturn = this.LoadGrid(userInfo, null, gridType);
            }

            return toReturn;
        }

        public Grid GetGridFromSearchId(InfoUtente userInfo, string gridId, Grid.GridTypeEnumeration gridType)
        {

            // Griglia restituita
            Grid toReturn = null;

            if (!string.IsNullOrEmpty(gridId) && gridId.Equals("-1"))
            {
                toReturn = this.LoadGrid(userInfo, null, gridType);
            }
            else
            {
                //Selezione della griglia standard per l'utente
                Query query = InitQuery.getInstance().getQuery(QueryNameForLoadGridFromSearchId);
                query.setParam("userId", userInfo.idPeople);
                query.setParam("roleId", userInfo.idGruppo);
                query.setParam("administrationId", userInfo.idAmministrazione);
                query.setParam("gridId", gridId);

                // Esecuzione della query
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GetGridFromSearchId - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GetGridFromSearchId - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    DataSet resultDataSet = new DataSet();
                    dbProvider.ExecuteQuery(resultDataSet, commandText);

                    //Se la griglia non viene trovata, si restituisce la griglia standard
                    if (resultDataSet.Tables[0].Rows.Count != 0)
                    {
                        XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                        MemoryStream stream = new MemoryStream(Convert.FromBase64String(resultDataSet.Tables[0].Rows[0]["SERIALIZED_GRID"].ToString()));
                        toReturn = (Grid)deserializer.Deserialize(stream);
                        toReturn.GridName = resultDataSet.Tables[0].Rows[0]["GRID_NAME"].ToString();
                    }
                }
                // else
                //     toReturn = GetStandardGridForUser(userInfo, gridType);

                //   toReturn.RapidSearchId = searchId;
            }
            return toReturn;
        }

        private bool ExistSavedGridForSavedSearch(String searchId, String userId, String roleId, String administrationId)
        {
            string rowNum = String.Empty;

            Query query = InitQuery.getInstance().getQuery(QueryNameForExistGridForSearchId);
            query.setParam("userId", userId);
            query.setParam("roleId", roleId);
            query.setParam("adminId", administrationId);
            query.setParam("searchId", searchId);

            using (DBProvider dbProvider = new DBProvider())
            {
                string commandText = query.getSQL();
                dbProvider.ExecuteScalar(out rowNum, commandText);
            }

            if (!String.IsNullOrEmpty(rowNum) && Int32.Parse(rowNum) > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Funzione per la creazione del filtro standard di ordinamento per documenti / fascicoli
        /// </summary>
        /// <param name="gridType">Tipo di oggetto associato alla griglia</param>
        /// <returns>Oggetto con le informazioni sul campo su cui compiere l'ordinamento</returns>
        private Field GetStandardFieldForOrder(Grid.GridTypeEnumeration gridType)
        {
            // Oggetto da restituire
            Field toReturn = null;

            switch (gridType)
            {
                case Grid.GridTypeEnumeration.Document:
                case Grid.GridTypeEnumeration.DocumentInProject:
                    toReturn = new Field()
                    {
                        Label = "Data protocollazione / Creazione",
                        OracleDbColumnName = "NVL (a.dta_proto, a.creation_time)",
                        SqlServerDbColumnName = "DTA_CREAZIONE",
                        FieldId = "D9",
                        OriginalLabel = "Data protocollazione / Creazione"
                    };

                    break;
                case Grid.GridTypeEnumeration.Project:
                    // Fascicoli -> DTA_CREAZIONE
                    toReturn = new Field()
                    {
                        Label = "Data creazione",
                        OracleDbColumnName = "A.DTA_CREAZIONE",
                        SqlServerDbColumnName = "A.DTA_CREAZIONE",
                        FieldId = "P20",
                        OriginalLabel = "Data creazione"
                    };
                    break;
                case Grid.GridTypeEnumeration.Transmission:
                    // Trasmissione --> null
                    toReturn = null;
                    break;
                case Grid.GridTypeEnumeration.NotRecognized:
                    break;
                default:
                    break;

            }

            // Restituzione del campo su cui compiere l'ordinamento
            return toReturn;

        }

        /// <summary>
        /// Questa funzione verifica se l'installazione del sistema prevede la funzione 
        /// di personalizzazione griglia
        /// </summary>
        /// <returns>True se la persinalizzazione griglie è attiva, false atrimenti</returns>
        public bool ExistGridPersonalizationFunction()
        {
            // Valore restituito dalla query e valore da restituire
            String result = String.Empty;

            // Reperimento della query e sua esecuzione
            Query query = InitQuery.getInstance().getQuery(QueryNameForExistGridPersonalizationFunction);

            // Reperimento del valore
            try
            {
                this.ExecuteScalar(out result, query.getSQL());

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la verifica di presenza della chiave GRID_PERSONALIZATION", e);
            }

            // Restituzione del valore
            return !String.IsNullOrEmpty(result) && result == "1";
        }

        /// <summary>
        /// Funzione per il restore della griglia standard relativa ad un utente per una specifica tipologia di ricerca
        /// </summary>
        /// <param name="userId">Identificativo dell'utente per cui ripristinare la griglia</param>
        /// <param name="roleId">Id del ruolo proprietario della griglia</param>
        /// <param name="administrationId">Identificativo dell'amministrazione cui appartiene l'utente</param>
        /// <param name="gridType">Tipo di griglia da restituire</param>
        /// <returns>Griglia standard</returns>
        /*   public Grid RestoreStandardGridForUser(String userId, String roleId, String administrationId, Grid.GridTypeEnumeration gridType)
           {
               // La griglia da restituire
               Grid standardGrid = this.GetEmergencyGrid(gridType);

               // Reperimento dell'id della griglia standard
               Query query = InitQuery.getInstance().getQuery(QueryNameForRetriveStandardGridId);
               query.setParam("gridType", gridType.ToString());
               query.setParam("userId", userId);
               query.setParam("roleId", roleId);
               query.setParam("administrationId", administrationId);

               String gridId;

               using (DocsPaDB.DBProvider dbProvider = new DBProvider())
               {
                   try
                   {
                       dbProvider.ExecuteScalar(out gridId, query.getSQL());
                       standardGrid.GridId = gridId;

                       // Salvataggio della griglia standard
                       this.SaveGrid(standardGrid, userId, roleId, administrationId);
                   }
                   catch (Exception)
                   {
                       logger.Debug("Errore durante il reperimento dell'id della griglia standard.");
                   }
               }


               // Restituzione della griglia
               return standardGrid;

           }*/

        /// <summary>
        /// Funzione per il caricamento delle informazioni di base relative alle griglie salvate per un dato utente
        /// appartenente ad un dato ruolo definito per una certa amministrazione e per un particolare tipo di ricerca.
        /// </summary>
        /// <param name="userId">Identificativo dell'utente</param>
        /// <param name="roleId">Identificativo del ruolo</param>
        /// <param name="administrationId">Identificativo dell'amministrazione</param>
        /// <param name="gridType">Tipo di griglia</param>
        /// <returns>Lista di oggetti con le informazioni di base sulle griglie  definite da un utente</returns>
        public List<GridBaseInfo> GetGridsBaseInfo(DocsPaVO.utente.InfoUtente infoUtente, Grid.GridTypeEnumeration gridType, List<GridBaseInfo> grids, bool allGrids)
        {

            try
            {
                Query query = null;
                if (allGrids)
                {
                    query = InitQuery.getInstance().getQuery(QueryNameForRetriveDefinedGrids);
                }
                else
                {
                    query = InitQuery.getInstance().getQuery(QueryNameForRetriveDefinedGridsOnlyRole);
                }
                query.setParam("userId", infoUtente.idPeople);
                query.setParam("roleId", infoUtente.idGruppo);
                query.setParam("administrationId", infoUtente.idAmministrazione);
                query.setParam("typeGrid", gridType.ToString());

                DataSet ds = new DataSet();
                this.ExecuteQuery(out ds, query.getSQL());
                Boolean isSearchGrid;
                Boolean isPreferred;
                Boolean role;
                Boolean user;
                Boolean almostOnePreferred = true;

                foreach (DataRow grid in ds.Tables[0].Rows)
                {

                    if (!string.IsNullOrEmpty(grid["IS_PREFERRED"].ToString()))
                    {
                        isPreferred = true;
                        almostOnePreferred = false;
                    }
                    else
                    {
                        isPreferred = false;
                    }

                    if ((grid["CHA_VISIBILE_A_UTENTE_O_RUOLO"].ToString()).Equals("R"))
                    {
                        role = true;
                    }
                    else
                    {
                        role = false;
                    }

                    if ((grid["IS_SEARCH_GRID"].ToString()).Equals("Y"))
                    {
                        isSearchGrid = true;
                    }
                    else
                    {
                        isSearchGrid = false;
                    }


                    if ((grid["CHA_VISIBILE_A_UTENTE_O_RUOLO"].ToString()).Equals("U"))
                    {
                        user = true;
                    }
                    else
                    {
                        user = false;
                    }

                    grids.Add(new GridBaseInfo()
                    {
                        GridName = grid["GRID_NAME"].ToString(),
                        GridId = grid["SYSTEM_ID"].ToString(),
                        IsPreferred = isPreferred,
                        IsSearchGrid = isSearchGrid,
                        RoleGrid = role,
                        UserGrid = user,
                        GridType = gridType.ToString()
                    });
                }

                grids.Insert(0, new GridBaseInfo()
                {
                    GridName = "Griglia standard",
                    GridId = "-1",
                    IsPreferred = almostOnePreferred,
                    IsSearchGrid = false,
                    RoleGrid = true,
                    UserGrid = true,
                    GridType = gridType.ToString()
                });

            }
            catch (Exception)
            {
                logger.Debug(String.Format("Errore durante il reperimento delle griglie definite dall'utente {0}", infoUtente.userId));
            }

            // Restituzione lista
            return grids;

        }

        public void RemoveGrid(String gridId)
        {

            // Reperimento dell'id della griglia standard
            Query query = InitQuery.getInstance().getQuery(QueryNameForRemoveGrid);
            query.setParam("gridId", gridId);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.ExecuteNonQuery(query.getSQL());
                }
                catch (Exception)
                {
                    logger.Debug("Errore durante la rimozione della griglia.");
                }
            }

        }

        public Grid GetUserGridCustom(InfoUtente userInfo, Grid.GridTypeEnumeration gridType)
        {
            // Griglia restituita
            Grid toReturn = null;

            //Selezione della griglia standard per l'utente
            Query query = InitQuery.getInstance().getQuery(QueryNameForUserGrid);
            query.setParam("idUser", userInfo.idPeople);
            query.setParam("idRole", userInfo.idGruppo);
            query.setParam("idAmm", userInfo.idAmministrazione);
            query.setParam("typeGrid", gridType.ToString());

            // Esecuzione della query
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetUserGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetUserGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                DataSet resultDataSet = new DataSet();
                dbProvider.ExecuteQuery(resultDataSet, commandText);

                if (resultDataSet.Tables[0].Rows.Count != 0)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(resultDataSet.Tables[0].Rows[0]["SERIALIZED_GRID"].ToString()));
                    toReturn = (Grid)deserializer.Deserialize(stream);
                    toReturn.GridId = resultDataSet.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    // toReturn.RapidSearchId = resultDataSet.Tables[0].Rows[0]["SEARCH_ID"].ToString();
                }
                else
                    toReturn = this.LoadGrid(userInfo, null, gridType);
            }

            return toReturn;
        }


        public bool RemoveUserGrid(DocsPaVO.Grid.GridBaseInfo gridBase, DocsPaVO.utente.InfoUtente userInfo)
        {
            bool result = false;
            // Reperimento dell'id della griglia standard
            Query query = InitQuery.getInstance().getQuery(QueryNameForRemoveGrid);
            query.setParam("gridId", gridBase.GridId);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.ExecuteNonQuery(query.getSQL());
                    result = true;
                }
                catch (Exception)
                {
                    logger.Debug("Errore durante la rimozione della griglia.");
                }
            }
            return result;

        }

        /// <summary>
        /// Questa funzione salva i dati relativi ad una nuova griglia
        /// Se isPreferred è a true viene inserito anche nella dpa_ass_grid
        /// </summary>
        /// <param name="grid">La griglia da salvare</param>
        /// <param name="userSystemId">Identificativo dell'utente proprietario della griglia</param>
        /// <param name="roleId">Identificativo della griglia</param>
        /// <param name="administrationId">Identificativo del ruolo cui appartiene l'utente proprietario della griglia</param>
        /// <returns>Id della griglia creata / modificata</returns>
        public String SaveNewGrid(
             Grid grid,
             InfoUtente infoUser,
             String gridName,
             String visibility,
             Boolean isPreferred)
        {
            // Id della griglia creata / modificata
            String gridId;

            if (!string.IsNullOrEmpty(visibility))
            {
                if (visibility.Equals("user"))
                {
                    visibility = "U";
                }
                else
                {
                    visibility = "R";
                }
            }

            gridId = this.InsertNewGridNew(grid, infoUser, gridName, visibility, isPreferred);



            // Restituzione dell'id della griglia
            return gridId;

        }

        /// <summary>
        /// Funzione per il salvataggio dei dati relativi ad una nuova griglia
        /// </summary>
        private String InsertNewGridNew(Grid grid, InfoUtente infoUser, String gridName, String visibility, Boolean isPreferred)
        {
            // System id della griglia creata
            String gridId;

            // Stringa con il risultato della serializzazione della griglia
            String serializedGrid;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                // Salvataggio delle informazioni sulla griglia sul DB, se la griglia serializzata è valorizzata
                Query query = InitQuery.getInstance().getQuery(QueryNameForSaveSettings);
                query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_GRIDS"));
                query.setParam("userId", infoUser.idPeople);
                query.setParam("roleId", infoUser.idGruppo);
                query.setParam("administrationId", infoUser.idAmministrazione);
                query.setParam("typeGrid", grid.GridType.ToString());
                query.setParam("gridName", gridName);
                query.setParam("searchGrid", "N");
                query.setParam("visibility", visibility);

                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);

                // Recupero id e inserimento campo CLOB
                string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_GRIDS");
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + sql);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + sql);
                dbProvider.ExecuteScalar(out gridId, sql);

                if (!string.IsNullOrEmpty(gridId))
                {
                    grid.GridId = gridId;

                    // File XML serializzato da salvare sul db
                    using (MemoryStream stream = new MemoryStream())
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                        mySerializer.Serialize(stream, grid);
                        stream.Position = 0;
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        serializedGrid = Convert.ToBase64String(data);
                    }

                    dbProvider.SetLargeText("DPA_GRIDS", gridId, "SERIALIZED_GRID", serializedGrid);
                }
            }

            //se ho impostato la griglia come preferita
            if (isPreferred)
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //cancello la vecchia preferita per la tipologia se presente in dpa_ass_grids
                    Query query = InitQuery.getInstance().getQuery(QueryNameForRemovePreferredGrid);
                    query.setParam("userId", infoUser.idPeople);
                    query.setParam("roleId", infoUser.idGruppo);
                    query.setParam("administrationId", infoUser.idAmministrazione);
                    query.setParam("typeGrid", grid.GridType.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - DeletePreferredGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    logger.Debug("SQL - DeletePreferredGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText);
                }

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //inserisco la nuova griglia in dpa_ass_grids
                    Query query = InitQuery.getInstance().getQuery(QueryNameForInsertPreferred);
                    query.setParam("gridId", gridId);
                    query.setParam("userId", infoUser.idPeople);
                    query.setParam("roleId", infoUser.idGruppo);
                    query.setParam("administrationId", infoUser.idAmministrazione);
                    query.setParam("typeGrid", grid.GridType.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertIntoDpaAssGrids - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertIntoDpaAssGrids - DocsPaDB/Grids.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText);
                }

            }

            return gridId;

        }

        public bool RemovePreferredGrid(DocsPaVO.Grid.GridBaseInfo gridBase, DocsPaVO.utente.InfoUtente userInfo)
        {
            bool result = false;
            // Reperimento dell'id della griglia standard
            Query query = InitQuery.getInstance().getQuery(QueryNameForRemovePreferredGridById);
            query.setParam("gridId", gridBase.GridId);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.ExecuteNonQuery(query.getSQL());
                    result = true;
                }
                catch (Exception)
                {
                    logger.Debug("Errore durante la rimozione della griglia.");
                }
            }
            return result;

        }

        public void RemovePreferredTypeGrid(InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType)
        {

            Query query = InitQuery.getInstance().getQuery(QueryNameForRemovePreferredGridByType);
            query.setParam("type", gridType.ToString());
            query.setParam("idPeople", infoUser.idPeople);
            query.setParam("idGruppo", infoUser.idGruppo);
            query.setParam("idAmm", infoUser.idAmministrazione);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.ExecuteNonQuery(query.getSQL());
                }
                catch (Exception)
                {
                    logger.Debug("Errore durante la rimozione della griglia preferita.");
                }
            }

        }

        public void AddPreferredGrid(String gridId, InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType)
        {
            RemovePreferredTypeGrid(infoUser, gridType);

            Query query = InitQuery.getInstance().getQuery(QueryNameForInsertPreferred);
            query.setParam("typeGrid", gridType.ToString());
            query.setParam("gridId", gridId);
            query.setParam("userId", infoUser.idPeople);
            query.setParam("roleId", infoUser.idGruppo);
            query.setParam("administrationId", infoUser.idAmministrazione);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.ExecuteNonQuery(query.getSQL());
                }
                catch (Exception)
                {
                    logger.Debug("Errore durante la rimozione della griglia preferita.");
                }
            }

        }

        /// <summary>
        /// Questa funzione modifica i dati di una griglia
        /// Se isPreferred è a true viene inserito anche nella dpa_ass_grid
        /// </summary>
        public String ModifyGrid(
             Grid grid,
             InfoUtente infoUser,
             String visibility,
             Boolean isPreferred)
        {
            // Id della griglia creata / modificata
            String gridId;

            if (!string.IsNullOrEmpty(visibility))
            {
                if (visibility.Equals("user"))
                {
                    visibility = "U";
                }
                else
                {
                    visibility = "R";
                }
            }

            gridId = this.ModifyOnlyGrid(grid, infoUser, visibility, isPreferred);



            // Restituzione dell'id della griglia
            return gridId;

        }

        /// <summary>
        /// Funzione per il salvataggio dei dati relativi ad una nuova griglia
        /// </summary>
        private String ModifyOnlyGrid(Grid grid, InfoUtente infoUser, String visibility, Boolean isPreferred)
        {
            string gridId = grid.GridId;

            // Stringa con il risultato della serializzazione della griglia
            String serializedGrid;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                // Salvataggio delle informazioni sulla griglia sul DB, se la griglia serializzata è valorizzata
                Query query = InitQuery.getInstance().getQuery(QueryNameForUpdateSavedGrid);
                query.setParam("administrationId", infoUser.idAmministrazione);
                query.setParam("gridId", gridId);
                query.setParam("visibility", visibility);
                query.setParam("gridName", grid.GridName);

                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);

                grid.GridId = gridId;

                // File XML serializzato da salvare sul db
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                    mySerializer.Serialize(stream, grid);
                    stream.Position = 0;
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    serializedGrid = Convert.ToBase64String(data);
                }

                dbProvider.SetLargeText("DPA_GRIDS", gridId, "SERIALIZED_GRID", serializedGrid);
            }

            //se ho impostato la griglia come preferita
            if (isPreferred)
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //cancello la vecchia preferita per la tipologia se presente in dpa_ass_grids
                    Query query = InitQuery.getInstance().getQuery(QueryNameForRemovePreferredGrid);
                    query.setParam("userId", infoUser.idPeople);
                    query.setParam("roleId", infoUser.idGruppo);
                    query.setParam("administrationId", infoUser.idAmministrazione);
                    query.setParam("typeGrid", grid.GridType.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - DeletePreferredGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    logger.Debug("SQL - DeletePreferredGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText);
                }

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //inserisco la nuova griglia in dpa_ass_grids
                    Query query = InitQuery.getInstance().getQuery(QueryNameForInsertPreferred);
                    query.setParam("gridId", gridId);
                    query.setParam("userId", infoUser.idPeople);
                    query.setParam("roleId", infoUser.idGruppo);
                    query.setParam("administrationId", infoUser.idAmministrazione);
                    query.setParam("typeGrid", grid.GridType.ToString());

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertIntoDpaAssGrids - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertIntoDpaAssGrids - DocsPaDB/Grids.cs - QUERY : " + commandText);

                    dbProvider.ExecuteNonQuery(commandText);
                }

            }

            return gridId;
        }

        /// <summary>
        /// Questa funzione salva i dati relativi ad una nuova griglia
        /// Se isPreferred è a true viene inserito anche nella dpa_ass_grid
        /// </summary>
        /// <param name="grid">La griglia da salvare</param>
        /// <param name="userSystemId">Identificativo dell'utente proprietario della griglia</param>
        /// <param name="roleId">Identificativo della griglia</param>
        /// <param name="administrationId">Identificativo del ruolo cui appartiene l'utente proprietario della griglia</param>
        /// <returns>Id della griglia creata / modificata</returns>
        public String SaveNewGridSearch(
             Grid grid,
             InfoUtente infoUser,
             String gridName, Grid.GridTypeEnumeration gridType)
        {
            // Id della griglia creata
            String gridId;

            gridName = "Ricerca salvata: " + gridName;

            gridId = this.InsertNewGridSearchId(grid, infoUser, gridName, gridType);



            // Restituzione dell'id della griglia
            return gridId;

        }

        /// <summary>
        /// Funzione per il salvataggio dei dati relativi ad una nuova griglia da salva ricerca
        /// </summary>
        private String InsertNewGridSearchId(Grid grid, InfoUtente infoUser, String gridName, Grid.GridTypeEnumeration gridType)
        {
            // System id della griglia creata
            String gridId;

            // Stringa con il risultato della serializzazione della griglia
            String serializedGrid;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                grid.GridId = string.Empty;
                // Salvataggio delle informazioni sulla griglia sul DB, se la griglia serializzata è valorizzata
                Query query = InitQuery.getInstance().getQuery(QueryNameForSaveSearchSettings);
                query.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_GRIDS"));
                query.setParam("userId", infoUser.idPeople);
                query.setParam("roleId", infoUser.idGruppo);
                query.setParam("administrationId", infoUser.idAmministrazione);
                query.setParam("typeGrid", gridType.ToString());
                query.setParam("gridName", gridName);
                query.setParam("searchGrid", "Y");

                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);

                // Recupero id e inserimento campo CLOB
                string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_GRIDS");
                System.Diagnostics.Debug.WriteLine("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + sql);
                logger.Debug("SQL - InsertNewGrid - DocsPaDB/Grids.cs - QUERY : " + sql);
                dbProvider.ExecuteScalar(out gridId, sql);

                if (!string.IsNullOrEmpty(gridId))
                {
                    grid.GridId = gridId;

                    // File XML serializzato da salvare sul db
                    using (MemoryStream stream = new MemoryStream())
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                        mySerializer.Serialize(stream, grid);
                        stream.Position = 0;
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        serializedGrid = Convert.ToBase64String(data);
                    }

                    dbProvider.SetLargeText("DPA_GRIDS", gridId, "SERIALIZED_GRID", serializedGrid);
                }
            }


            return gridId;

        }


        public void refreshAllGrid()
        {

            Query query = InitQuery.getInstance().getQuery(QueryNameForRefreshGrid);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - GET_ALL_GRIDS - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    logger.Debug("SQL - GET_ALL_GRIDS - DocsPaDB/Grids.cs - QUERY : " + commandText);
                    DataSet resultDataSet = new DataSet();
                    dbProvider.ExecuteQuery(resultDataSet, commandText);

                    if (resultDataSet.Tables[0] != null && resultDataSet.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow tempRow in resultDataSet.Tables[0].Rows)
                        {
                            XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                            MemoryStream stream = new MemoryStream(Convert.FromBase64String(tempRow["SERIALIZED_GRID"].ToString()));
                            Grid tempGrid = (Grid)deserializer.Deserialize(stream);

                            string typeGrid = tempRow["TYPE_GRID"].ToString();
                            List<Field> newField = new List<Field>();
                            if (typeGrid.Equals("Project"))
                            {
                                newField = GetStandardFieldForProject();
                            }
                            else
                            {
                                newField = GetStandardFieldForDocument();
                            }

                            foreach (Field tempField in newField)
                            {
                                Field d = (Field)(tempGrid.Fields.Where(e => e.FieldId.Equals(tempField.FieldId)).FirstOrDefault());
                                if (d != null)
                                {
                                    d.OriginalLabel = tempField.OriginalLabel;
                                    d.SqlServerDbColumnName = tempField.SqlServerDbColumnName;
                                    d.OracleDbColumnName = tempField.OracleDbColumnName;
                                }
                                else
                                {
                                    tempGrid.Fields.Add(tempField);
                                }
                            }

                            String serializedGrid;

                            // File XML serializzato da salvare sul db
                            using (MemoryStream streamUp = new MemoryStream())
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                                mySerializer.Serialize(streamUp, tempGrid);
                                streamUp.Position = 0;
                                byte[] data = new byte[streamUp.Length];
                                streamUp.Read(data, 0, data.Length);
                                serializedGrid = Convert.ToBase64String(data);
                            }

                            dbProvider.SetLargeText("DPA_GRIDS", tempGrid.GridId, "SERIALIZED_GRID", serializedGrid);

                        }

                    }
                }
                catch (Exception)
                {
                    logger.Debug("Errore durante il refresh delle griglie.");
                }
            }

        }

        /// <summary>
        /// Ultima nota oppure si/no
        /// </summary>
        /// <returns></returns>
        public bool IsPresentsNote()
        {
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                return value == "1";
            }
            else
            {
                return false;
            }
        }

    }
}
