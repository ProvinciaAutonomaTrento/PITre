// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CorrGlobaliEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_RF { get; set; }
        public long? INTEROPRFID { get; set; }
        public long? INTEROPREGISTRYID { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_OLD { get; set; }
        public long? ID_PARENT { get; set; }
        public long? NUM_LIVELLO { get; set; }
        public long? ID_GRUPPO { get; set; }
        public long? ID_TIPO_RUOLO { get; set; }
        public long? ID_UO { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_PESO { get; set; }
        public long? NUM_FIGLI { get; set; }
        public long? NUM_PORTA_SMTP { get; set; }
        public long? ID_GRUPPO_LISTE { get; set; }
        public long? ID_PESO_ORG { get; set; }
        public long? ORIGINAL_ID { get; set; }
        public long? ID_PEOPLE_LISTE { get; set; }
        public DateTime? DTA_INIZIO { get; set; }
        public DateTime? DTA_FINE { get; set; }
        public string? CHA_DEFAULT_TRASM { get; set; }
        public string? CHA_TIPO_CORR { get; set; }
        public string? CHA_TIPO_IE { get; set; }
        public string? CHA_TIPO_URP { get; set; }
        public string? CHA_PA { get; set; }
        public string? CHA_DETTAGLI { get; set; }
        public string? CHA_RIFERIMENTO { get; set; }
        public string? CHA_RESPONSABILE { get; set; }
        public string? AOO_SOSPESO { get; set; }
        public string? CHA_SEGRETARIO { get; set; }
        public string? VAR_INSERT_BY_INTEROP { get; set; }
        public string? CHA_SYSTEM_ROLE { get; set; }
        public string? RUBRICA_ESTERNA { get; set; }
        public string? VAR_CHIAVE_AE { get; set; }
        public string? VAR_COD_RUBRICA { get; set; }
        public string? VAR_CODICE { get; set; }
        public string? VAR_EMAIL { get; set; }
        public string? VAR_SMTP { get; set; }
        public string? VAR_ORIGINAL_CODE { get; set; }
        public string? VAR_CODICE_AOO { get; set; }
        public string? VAR_DESC_CORR { get; set; }
        public string? VAR_DESC_CORR_OLD { get; set; }
        public string? VAR_CODICE_AMM { get; set; }
        public string? VAR_CODICE_ISTAT { get; set; }
        public string? INTEROPURL { get; set; }
        public string? VAR_COGNOME { get; set; }
        public string? VAR_NOME { get; set; }
        public string? CLASSIFICA_UO { get; set; }
        public string? COD_DESC_INTEROP { get; set; }
        public string? VAR_FAX_USER_LOGIN { get; set; }
        public string? CHA_DISABLED_TRASM { get; set; }
    }
}
