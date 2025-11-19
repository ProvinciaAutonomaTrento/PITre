// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ProfileEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? DOCNUMBER { get; set; }
        public string? DOCNAME { get; set; }
        public long? TYPIST { get; set; }
        public long? AUTHOR { get; set; }
        public long? DOCUMENTTYPE { get; set; }
        public long? LAST_EDITED_BY { get; set; }
        public long? LAST_LOCKED_BY { get; set; }
        public long? LAST_ACCESS_ID { get; set; }
        public DateTime? PROCESS_DATE { get; set; }
        public DateTime? CREATION_DATE { get; set; }
        public DateTime? CREATION_TIME { get; set; }
        public DateTime? LAST_EDIT_DATE { get; set; }
        public DateTime? LAST_EDIT_TIME { get; set; }
        public DateTime? LAST_ACCESS_DATE { get; set; }
        public DateTime? LAST_ACCESS_TIME { get; set; }
        public DateTime? ARCHIVE_DATE { get; set; }
        public long? STATUS { get; set; }
        public string? PATH { get; set; }
        public string? DOCSERVER_LOC { get; set; }
        public long? ID_REGISTRO { get; set; }
        public string? CHA_TIPO_PROTO { get; set; }
        public long? ID_OGGETTO { get; set; }
        public long? NUM_PROTO { get; set; }
        public long? NUM_ANNO_PROTO { get; set; }
        public string? VAR_PROTO_EME { get; set; }
        public DateTime? DTA_PROTO_EME { get; set; }
        public string? VAR_COGNOME_EME { get; set; }
        public string? VAR_NOME_EME { get; set; }
        public long? ID_PARENT { get; set; }
        public DateTime? DTA_PROTO { get; set; }
        public string? CHA_MOD_OGGETTO { get; set; }
        public string? CHA_MOD_MITT_DEST { get; set; }
        public string? CHA_MOD_MITT_INT { get; set; }
        public string? CHA_MOD_DEST_OCC { get; set; }
        public DateTime? DTA_PROTO_IN { get; set; }
        public string? VAR_PROTO_IN { get; set; }
        public long? ID_ANNULLATORE { get; set; }
        public DateTime? DTA_ANNULLA { get; set; }
        public string? VAR_AUT_ANNULLA { get; set; }
        public string? VAR_SEGNATURA { get; set; }
        public string? CHA_DA_PROTO { get; set; }
        public string? VAR_NOTE { get; set; }
        public long? ID_TIPO_ATTO { get; set; }
        public string? CHA_ASSEGNATO { get; set; }
        public string? CHA_IMG { get; set; }
        public string? CHA_FASCICOLATO { get; set; }
        public string? CHA_INVIO_CONFERMA { get; set; }
        public string? CHA_CONGELATO { get; set; }
        public string? CHA_CONSOLIDATO { get; set; }
        public string? CHA_PRIVATO { get; set; }
        public string VAR_CHIAVE_PROTO { get; set; }
        public string? VAR_NUM_OGGETTO { get; set; }
        public string? VAR_COMM_REF { get; set; }
        public string? CHA_EVIDENZA { get; set; }
        public long? APPLICATION { get; set; }
        public string? VAR_SEDE { get; set; }
        public string? VAR_PROF_OGGETTO { get; set; }
        public long? ID_PEOPLE_PROT { get; set; }
        public long? ID_RUOLO_PROT { get; set; }
        public long? ID_UO_PROT { get; set; }
        public long? ID_UO_REF { get; set; }
        public long? ID_RUOLO_CREATORE { get; set; }
        public long? ID_UO_CREATORE { get; set; }
        public string? CHA_INTEROP { get; set; }
        public DateTime? DTA_SCADENZA { get; set; }
        public string? CHA_PERSONALE { get; set; }
        public string? CHA_IN_CESTINO { get; set; }
        public string? VAR_NOTE_CESTINO { get; set; }
        public long? ID_DOCUMENTO_PRINCIPALE { get; set; }
        public string? CHA_IN_ARCHIVIO { get; set; }
        public string CHA_FIRMATO { get; set; }
        public string? PROT_TIT { get; set; }
        public long? NUM_IN_FASC { get; set; }
        public long? ID_FASC_PROT_TIT { get; set; }
        public long? NUM_PROT_TIT { get; set; }
        public long? ID_TITOLARIO { get; set; }
        public DateTime? DTA_PROTO_TIT { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
        public string? CHA_RIFF_MITT { get; set; }
        public string? CHA_DOCUMENTO_DA_PEC { get; set; }
        public long? LAST_FORWARD { get; set; }
        public string? CHA_UNLOCKED_FINAL_STATE { get; set; }
        public string? CONSOLIDATION_STATE { get; set; }
        public long? CONSOLIDATION_AUTHOR { get; set; }
        public long? CONSOLIDATION_ROLE { get; set; }
        public DateTime? CONSOLIDATION_DATE { get; set; }
        public long? FORWARDING_SOURCE { get; set; }
        public long? PRINTS_NUM { get; set; }
        public string? CHA_COD_T_A { get; set; }
        public string? COD_EXT_APP { get; set; }
        public string? ID_VECCHIO_DOCUMENTO { get; set; }
        public string? EXT { get; set; }
        public string? IN_LIBROFIRMA { get; set; }
        public string? CHA_TASK_STATUS { get; set; }
    }
}
