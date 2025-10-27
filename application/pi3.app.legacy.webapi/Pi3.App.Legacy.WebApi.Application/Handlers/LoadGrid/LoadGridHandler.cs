// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Grid;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LoadGridRequest = Pi3.App.Legacy.WebApi.Application.Requests.LoadGrid;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.Mobile;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.LoadGrid
{
    public class LoadGridHandler : IRequestHandler<LoadGridRequest, LoadGridResult>
    {
        protected readonly ILogger<LoadGridHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;

        public LoadGridHandler(
            ILogger<LoadGridHandler> logger,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IMediator mediator
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._configurationService = configurationService;
            this._mediator = mediator;
        }


        private Field GetFieldProperties(string fieldName, string fieldLabel, String id, bool canAssumeMultipleValue, int customObjectId, int position, bool visible, String dbOracleColumnName, String dbSQLColumnName, String templateName, int fieldWidth)
        {
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

            return toReturn;

        }
        private async Task<bool> IsPresentsNote()
        {
            
            var valoreChiaveAtipicita = await this._configurationService.GetValue<string>("0", "FE_IS_PRESENT_NOTE");



            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
            {
                return valoreChiaveAtipicita == "1";
            }
            else
            {
                return false;
            }
        }

        private async Task<List<Field>> GetStandardFieldForDocument()
        {
            List<Field> toReturn = new List<Field>();

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
            if (await IsPresentsNote())
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

            var valoreChiaveAtipicita = await this._configurationService.GetValue<string>("0", "ATIPICITA_DOC_FASC");

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
            toReturn.Add(this.GetFieldProperties(
                "Esito Spedizione",
                "Esito Spedizione",
                "esito_spedizione",
                false, 0,
                28,
                false,
                "getEsitoSpedizione(a.system_id)",
                "@dbuser@.getEsitoSpedizione(a.system_id)",
                "Standard",
                200));
            toReturn.Add(this.GetFieldProperties(
                "Num. Ricevute",
                "Num. Ricevute",
                "count_ric_interop",
                false, 0,
                29,
                false,
                "getCountRicevuteInterop(a.system_id, @tipoRicevutaInteroperante@)",
                "@dbuser@.getCountRicevuteInterop(a.system_id, @tipoRicevutaInteroperante@)",
                "Standard",
                100));

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

            toReturn.Add(this.GetFieldProperties(
                  "Codice protocollatore",
                  "Codice protocollatore",
                  "D26",
                  false,
                  0,
                  36,
                  false,
                  "UPPER(trim(getpeopleuserid(a.id_people_prot)))",
                    "UPPER(LTRIM(RTRIM(@dbuser@.getPeopleUserId(A.id_people_prot))))",
                  "Standard",
                  100));

            toReturn.Add(this.GetFieldProperties(
                  "Nome e cognome protocollatore",
                  "Nome e cognome protocollatore",
                  "D27",
                  false,
                  0,
                  37,
                  false,
                  "getpeoplename (a.id_people_prot)",
                  "@dbuser@.getpeoplename (a.id_people_prot)",
                  "Standard",
                  200));

            toReturn.Add(this.GetFieldProperties(
                    "Ruolo protocollatore",
                    "Ruolo protocollatore",
                    "D28",
                    false,
                    0,
                    37,
                    false,
                    "getdesccorr(a.id_ruolo_prot)",
                    "@dbuser@.getdesccorr(a.id_ruolo_prot)",
                    "Standard",
                    200));

            return toReturn;
        }

        private async Task<List<Field>> GetStandardFieldForProject()
        {
            List<Field> toReturn = new List<Field>();


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

            if (await IsPresentsNote())
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
            "Campi profilati fascicolo",
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


            var valoreChiaveAtipicita = await this._configurationService.GetValue<string>("0", "ATIPICITA_DOC_FASC");


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

            toReturn.Add(this.GetFieldProperties(
                   "Tipologia fascicolo",
                   "Tipologia fascicolo",
                   "TIPOLOGIA_FASCICOLO",
                   false,
                   0,
                   26,
                   false,
                   "GetTipologiaFascicoloPianoCons(a.id_piano_conservazione)",
                    "@dbuser@.GetTipologiaFascicoloPianoCons (a.id_piano_conservazione)",
                   "Standard",
                   200));

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

            // Restituzione della lista
            return toReturn;
        }

        private List<Field> GetStandardGridForTransmission()
        {

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

            return toReturn;
        }

        private async Task<List<Field>> GetStandardGrid(Grid.GridTypeEnumeration gridType)
        {
            List<Field> toReturn = new List<Field>();

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

            switch (gridType)
            {
                case Grid.GridTypeEnumeration.Document:
                case Grid.GridTypeEnumeration.DocumentInProject:
                    toReturn.AddRange(await this.GetStandardFieldForDocument());
                    break;
                case Grid.GridTypeEnumeration.Project:
                    toReturn.AddRange(await this.GetStandardFieldForProject());
                    break;
                case Grid.GridTypeEnumeration.Transmission:
                    toReturn.AddRange(this.GetStandardGridForTransmission());
                    break;

            }

            int i = toReturn.Count;


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

            return toReturn;

        }

        private async Task<List<Field>> GetTemplateField(String templateId, string roleId, string administrationId, Grid.GridTypeEnumeration gridType)
        {
            List<Field> toReturn;

            Templates template = null;

            AssDocFascRuoli[] visibilityInformation = null;
            toReturn = new List<Field>();

            switch (gridType)
            {
                case Grid.GridTypeEnumeration.Document:
                case Grid.GridTypeEnumeration.DocumentInProject:

                    var respModel = await this._mediator.Send(new Application.Requests.getTemplateById(templateId));
                    template = respModel.output;
                    visibilityInformation = (await this._mediator.Send(new Application.Requests.getDirittiCampiTipologiaDoc(roleId, templateId))).output;
                    break;
                case Grid.GridTypeEnumeration.Project:

                    var respFascModel = await this._mediator.Send(new Application.Requests.getTemplateFascById(templateId));
                    template = respFascModel.output;

                    visibilityInformation = (await this._mediator.Send(new Application.Requests.getDirittiCampiTipologiaFasc(roleId, templateId))).output;


                    break;
            }

            if (visibilityInformation != null)
                visibilityInformation = visibilityInformation.Where(e => e.VIS_OGG_CUSTOM == "1").ToArray();

            int nextId = toReturn.Count;
            OggettoCustom[] customObjects;

            if (template != null)
            {
                customObjects = template.ELENCO_OGGETTI;
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

            return toReturn;

        }

        private Field GetStandardFieldForOrder(Grid.GridTypeEnumeration gridType)
        {

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

                    toReturn = null;
                    break;
                case Grid.GridTypeEnumeration.NotRecognized:
                    break;
                default:
                    break;

            }


            return toReturn;


        }
        private async Task<Grid> LoadGrid(InfoUtente userInfo, List<String> templatesId, Grid.GridTypeEnumeration gridType)
        {
            Grid result = new Grid("-1", templatesId, gridType);
            result.Fields.AddRange(await this.GetStandardGrid(gridType));

            if (templatesId != null)
                foreach (String templateId in templatesId)
                    result.Fields.AddRange(
                        await this.GetTemplateField(templateId, userInfo.idGruppo, userInfo.idAmministrazione, gridType));

            if (result.FieldForOrder == null)
            {
                result.FieldForOrder = this.GetStandardFieldForOrder(gridType);
                result.OrderDirection = Grid.OrderDirectionEnum.Desc;
            }

            result.ColorForFieldWithotTemplate = "990000";


            return result;

        }



        private async Task<Grid?> LoadGrid(string gridId)
        {
            Grid? result = null;

            GridEntity? grid = await this._dbContext.GridEntities.AsNoTracking().FirstOrDefaultAsync(grid => grid.SYSTEM_ID == gridId.AsLong());


            if (grid != null)
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                if (grid.SERIALIZED_GRID != null)
                {

                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(grid.SERIALIZED_GRID));
                    result = (Grid)deserializer.Deserialize(stream);
                }
            }
            return result;

        }

        private async Task<Grid> GetEmergencyGrid(Grid.GridTypeEnumeration gridType)
        {
            Grid result = new Grid(String.Empty, null, gridType);

            List<Field> toReturn = new List<Field>();

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

            switch (gridType)
            {
                case Grid.GridTypeEnumeration.Document:
                case Grid.GridTypeEnumeration.DocumentInProject:
                    toReturn.AddRange(await this.GetStandardFieldForDocument());
                    break;
                case Grid.GridTypeEnumeration.Project:
                    toReturn.AddRange(await this.GetStandardFieldForProject());
                    break;
                case Grid.GridTypeEnumeration.Transmission:
                    toReturn.AddRange(this.GetStandardGridForTransmission());
                    break;

            }

            int i = toReturn.Count;


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

            result.Fields = toReturn;
            result.FieldForOrder = this.GetStandardFieldForOrder(gridType);
            result.OrderDirection = Grid.OrderDirectionEnum.Desc;


            return result;
        }

        public async Task<LoadGridResult> Handle(LoadGridRequest request,CancellationToken cancellationToken)
        {
            Grid output = new Grid();

            try
            {
                if (!string.IsNullOrEmpty(request.gridId) && !request.gridId.Equals("-1"))
                {
                    output = await this.LoadGrid(request.gridId) ?? output;
                }
                else
                {
                    output = await this.LoadGrid(request.userInfo, request.templatesId, request.gridType);
                }
                if (request.getEmergencyGrid)
                {
                    output = await GetEmergencyGrid(request.gridType);
                }
            }
            catch(Exception e)
            {
                this._logger.LogWebMethodError(e);
                output = await GetEmergencyGrid(request.gridType);
            }

            return new LoadGridResult(output);
        }



    }
}
