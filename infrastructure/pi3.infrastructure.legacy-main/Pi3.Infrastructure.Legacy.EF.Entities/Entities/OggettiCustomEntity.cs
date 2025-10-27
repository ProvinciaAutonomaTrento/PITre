// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class OggettiCustomEntity
    {
        public long SYSTEM_ID { get; set; }

        public string? DESCRIZIONE { get; set; }

        public string? ORIZZONTALE_VERTICALE { get; set; }

        public string? CAMPO_OBBLIGATORIO { get; set; }

        public string? MULTILINEA { get; set; }

        public string? NUMERO_DI_LINEE { get; set; }

        public string? NUMERO_DI_CARATTERI { get; set; }

        public string? CAMPO_DI_RICERCA { get; set; }

        public long? ID_TIPO_OGGETTO { get; set; }

        public string? RESET_ANNO { get; set; }

        public string? FORMATO_CONTATORE { get; set; }

        public string? ID_R_DEFAULT { get; set; }

        public string? RICERCA_CORR { get; set; }

        public string? CHA_TIPO_TAR { get; set; }

        public int? CONTA_DOPO { get; set; }

        public int? REPERTORIO { get; set; }

        public int? CAMPO_COMUNE { get; set; }

        public int? DA_VISUALIZZARE_RICERCA { get; set; }

        public string? FORMATO_ORA { get; set; }

        public string? TIPO_LINK { get; set; }

        public string? TIPO_OBJ_LINK { get; set; }

        public string? CONFIG_OBJ_EST { get; set; }

        public int? MODULO_SOTTOCONTATORE { get; set; }

        public string? CHA_CONSOLIDAMENTO { get; set; }

        public string? CHA_CONSERVAZIONE { get; set; }

        public string? CHA_CONS_REPERTORIO { get; set; }
    }
}
