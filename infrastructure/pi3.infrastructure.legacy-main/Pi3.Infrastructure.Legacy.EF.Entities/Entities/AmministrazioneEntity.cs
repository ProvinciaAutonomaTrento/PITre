// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AmministrazioneEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? VAR_CODICE_AMM { get; set; }
        public string? VAR_CODICE_ISTAT { get; set; }
        public string? VAR_DESC_AMM { get; set; }
        public string? VAR_LIBRERIA { get; set; }
        public string? CHA_SEPARATORE { get; set; }
        public string? CHA_STR_FISSA { get; set; }
        public string? CHA_STR_SEGNATURA { get; set; }
        public string? VAR_SMTP { get; set; }
        public long? NUM_PORTA_SMTP { get; set; }
        public string? VAR_LOGIN { get; set; }
        public string? VAR_PATH { get; set; }
        public string? VAR_FORMATO_SEGNATURA { get; set; }
        public string? VAR_FORMATO_FASCICOLATURA { get; set; }
        public string? VAR_DOMINIO { get; set; }
        public string? CHA_PROTOINT { get; set; }
        public string? VAR_USER_SMTP { get; set; }
        public string? VAR_PWD_SMTP { get; set; }
        public string? VAR_RAGIONE_REFERENTE { get; set; }
        public long? ID_RAGIONE_TO { get; set; }
        public long? ID_RAGIONE_CC { get; set; }
        public long? ID_RAGIONE_REFERENTE { get; set; }
        public long? NUM_GG_PERM_TODOLIST { get; set; }
        public string? CHA_ATTIVA_GG_PERM_TODOLIST { get; set; }
        public string? CHA_SMTP_SSL { get; set; }
        public string? CHA_SMTP_STA { get; set; }
        public string? ENABLE_PASSWORD_EXPIRATION { get; set; }
        public string? PASSWORD_EXPIRATION_DAYS { get; set; }
        public string? PASSWORD_MIN_LENGTH { get; set; }
        public string? PASSWORD_SPECIAL_CHAR_LIST { get; set; }
        public string? FROM_EMAIL_ADDRESS { get; set; }
        public long? ID_RAGIONE_CONOSCENZA { get; set; }
        public long? ID_RAGIONE_COMPETENZA { get; set; }
        public string? FONTCOLOR { get; set; }
        public string? VAR_FORMATO_TIMBRO { get; set; }
        public long? ID_CARAT_DF { get; set; }
        public long? ID_COLORE_DF { get; set; }
        public long? ID_POS_DF { get; set; }
        public string? ORIENTAMENTO { get; set; }
        public string? TIPO_ROTAZ { get; set; }
        public long? NUM_RECORD { get; set; }
        public string? CHA_ARCHIVIAZIONE_LOG { get; set; }
        public string? PULS_COLOR { get; set; }
        public string? VAR_FORMATO_DOMINIO { get; set; }
        public long? ID_CLIENT_MODEL_PROCESSOR { get; set; }
        public long? CSS { get; set; }
        public string? NEWS { get; set; }
        public string? ENABLE_NEWS { get; set; }
        public string? VAR_FORMATO_PROT_TIT { get; set; }
        public string? COL_SEGN { get; set; }
        public string? SPEDIZIONE_AUTO_DOC { get; set; }
        public string? AVVISA_SPEDIZIONE_DOC { get; set; }
        public string? TIPO_DOC_OBBL { get; set; }
        public string? IS_ENABLED_SMART_CLIENT { get; set; }
        public string? SMART_CLIENT_PDF_CONV_ON_SCAN { get; set; }
        public long? ID_DISPOSITIVO_STAMPA { get; set; }
        public string? TRASMISSIONE_AUTO_DOC { get; set; }
        public string CHA_TIPO_COMPONENTI { get; set; }
        public string? VAR_DETTAGLIO_FIRMA { get; set; }
        public string? VAR_EMAIL_RES_IPA { get; set; }
        public string? VAR_CODICE_AMM_IPA { get; set; }
        public string? TIPO_COMPONENTI { get; set; }
        public long? ID_RUOLO_RESP_CONS { get; set; }
        public long? ID_UTENTE_RESP_CONS { get; set; }
        public string? VAR_MSG_BANNER { get; set; }
        public string? VAR_INDIRIZZO_DIGITALE_RIF { get; set; }
        public string? CHA_ENABLE_CONS { get; set; }
        public string? PROVIDER_ID { get; set; }
        public string? MS_TENANT_ID { get; set; }
        public string? MS_CLIENT_ID { get; set; }
        public string? MS_CLIENT_SEC { get; set; }

    }
}
