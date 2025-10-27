// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ProjectEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? DESCRIPTION { get; set; }
        public string? ICONIZED { get; set; }
        public string? CHA_TIPO_PROJ { get; set; }
        public string? VAR_CODICE { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? NUM_LIVELLO { get; set; }
        public string? CHA_TIPO_FASCICOLO { get; set; }
        public long? ID_FASCICOLO { get; set; }
        public long? ID_PARENT { get; set; }
        public string? VAR_NOTE { get; set; }
        public DateTime? DTA_APERTURA { get; set; }
        public DateTime? DTA_CHIUSURA { get; set; }
        public string? CHA_STATO { get; set; }
        public string? VAR_COD_ULTIMO { get; set; }
        public string? VAR_COD_LIV1 { get; set; }
        public string? ET_TITOLARIO { get; set; }
        public string? ET_LIVELLO1 { get; set; }
        public string? ET_LIVELLO2 { get; set; }
        public string? ET_LIVELLO3 { get; set; }
        public string? ET_LIVELLO4 { get; set; }
        public string? ET_LIVELLO5 { get; set; }
        public string? ET_LIVELLO6 { get; set; }
        public long? ID_TIPO_PROC { get; set; }
        public long? ID_LEGISLATURA { get; set; }
        public long? ETDOC_RANDOM_ID { get; set; }
        public DateTime? DTA_CREAZIONE { get; set; }
        public long? NUM_FASCICOLO { get; set; }
        public long? ANNO_CREAZIONE { get; set; }
        public string? CHA_RW { get; set; }
        public long? ID_UO_REF { get; set; }
        public long? ID_UO_LF { get; set; }
        public DateTime? DTA_UO_LF { get; set; }
        public long? NUM_MESI_CONSERVAZIONE { get; set; }
        public string VAR_CHIAVE_FASC { get; set; }
        public string? CARTACEO { get; set; }
        public string? CHA_PRIVATO { get; set; }
        public long? ID_TIPO_FASC { get; set; }
        public string? CHA_BLOCCA_FASC { get; set; }
        public long? ID_TITOLARIO { get; set; }
        public DateTime? DTA_ATTIVAZIONE { get; set; }
        public DateTime? DTA_CESSAZIONE { get; set; }
        public DateTime? DTA_SCADENZA { get; set; }
        public string? CHA_IN_ARCHIVIO { get; set; }
        public long? AUTHOR { get; set; }
        public long? ID_RUOLO_CREATORE { get; set; }
        public long? ID_UO_CREATORE { get; set; }
        public string? CHA_BLOCCA_FIGLI { get; set; }
        public string? CHA_CONTA_PROT_TIT { get; set; }
        public string? NUM_PROT_TIT { get; set; }
        public string? MAX_LIV_TIT { get; set; }
        public string? ID_PEOPLE_DELEGATO { get; set; }
        public string? CHA_CONTROLLATO { get; set; }
        public string CHA_CONSENTI_CLASS { get; set; }
        public long? ID_RUOLO_CHIUSURA { get; set; }
        public long? ID_UO_CHIUSURA { get; set; }
        public long? ID_AUTHOR_CHIUSURA { get; set; }
        public string? CHA_COD_T_A { get; set; }
        public string? COD_EXT_APP { get; set; }
        public string? CHA_IN_CESTINO { get; set; }
        public string? CHA_CONSENTI_FASC { get; set; }
        public string? CHA_PUBBLICO { get; set; }
        public long? ID_PIANO_CONSERVAZIONE { get; set; }

    }
}
