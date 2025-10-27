// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class MvDocumentiCustomEntity
    {
        public string? ASSTROWID { get; set; }

        public string? OGGCROWID { get; set; }

        public string? OGGCCROWID { get; set; }

        public string? TAROWID { get; set; }

        public string? TOROWID { get; set; }

        public long? SYSTEM_ID_TEMPLATE { get; set; }

        public long? SYSTEM_ID_OGG_CUSTOM { get; set; }

        public long SYSTEM_ID_OGG_CUSTOM_COMP { get; set; }

        public long? SYSTEM_ID_TIPO_OGGETTO { get; set; }

        public long? SYSTEM_ID_TIPO_ATTO { get; set; }

        public string? VAR_DESC_ATTO { get; set; }

        public long? ABILITATO_SI_NO { get; set; }

        public string? IN_ESERCIZIO { get; set; }

        public string? PATH_MOD_1 { get; set; }

        public string? PATH_MOD_2 { get; set; }

        public string? PATH_MOD_SU { get; set; }

        public string? PATH_MOD_EXC { get; set; }

        public string? PATH_ALL_1 { get; set; }

        public long? GG_SCADENZA { get; set; }

        public long? GG_PRE_SCADENZA { get; set; }

        public string? CHA_PRIVATO { get; set; }

        public long? ID_AMM { get; set; }

        public string? COD_CLASS { get; set; }

        public string? COD_MOD_TRASM { get; set; }

        public long? IPERDOCUMENTO { get; set; }

        public string? DOC_NUMBER { get; set; }

        public string? VALORE_OGGETTO_DB { get; set; }

        public string? CODICE_DB { get; set; }

        public long? MANUAL_INSERT { get; set; }

        public long? ANNO { get; set; }

        public long? ID_AOO_RF { get; set; }

        public long? VALORE_SC { get; set; }

        public DateTime? DTA_INS { get; set; }

        public DateTime? DTA_ANNULLAMENTO { get; set; }

        public string? CAMPO_DI_RICERCA { get; set; }

        public string? CAMPO_OBBLIGATORIO { get; set; }

        public string? DESCRIZIONE { get; set; }

        public string? MULTILINEA { get; set; }

        public string? NUMERO_DI_CARATTERI { get; set; }

        public string? NUMERO_DI_LINEE { get; set; }

        public string? ORIZZONTALE_VERTICALE { get; set; }

        public string? RESET_ANNO { get; set; }

        public string? FORMATO_CONTATORE { get; set; }

        public string? RICERCA_CORR { get; set; }

        public string? ID_R_DEFAULT { get; set; }

        public long? CAMPO_COMUNE { get; set; }

        public string? CHA_TIPO_TAR { get; set; }

        public long? CONTA_DOPO { get; set; }

        public long? REPERTORIO { get; set; }

        public long? DA_VISUALIZZARE_RICERCA { get; set; }

        public string? FORMATO_ORA { get; set; }

        public string? TIPO_LINK { get; set; }

        public string? TIPO_OBJ_LINK { get; set; }

        public long? MODULO_SOTTOCONTATORE { get; set; }

        public string? ENABLEDHISTORY { get; set; }

        public long? POSIZIONE { get; set; }

        public long? ID_TEMPLATE { get; set; }

        public string? DESCRIZIONE_TIPO { get; set; }
    }
}
