// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.Services.StampaRepertori
{
    public class RepertorioPrintRange
    {
        /// <summary>
        /// Anno cui si riferisce l'intervallo di stampa
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Estremo inferiore del range
        /// </summary>
        public int FirstNumber { get; set; }

        /// <summary>
        /// Estremo inferiore del range
        /// </summary>
        public int LastNumber { get; set; }
    }

    public class ReportRepertoriItem : ValueObject
    {
        public string? IdDoc { get; init; }

        public string? Oggetto { get; init; }

        public string? IsModificato { get; init; }

        public string? DescrizioneCampo { get; init; }

        public string? ValoreCampo { get; init; }

        public long? IdAmm { get; init; }

        public DateTime? DataInserimento { get; init; }

        public DateTime? DataAnnullamento { get; init; }

        public long? DocNumber { get; init; }

        public string? ObjType { get; init; }

        public string? EnabledHistory { get; init; }

        public long? IdObject { get; init; }

        public long? CampoComune { get; init; }

        public string? Tipologia { get; init; }

        public string? SegnaturaRepertorio { get; set; }
        public DateTime? DataRepertorio { get; set; }

        public string? Impronta { get; set; }

    }
}
