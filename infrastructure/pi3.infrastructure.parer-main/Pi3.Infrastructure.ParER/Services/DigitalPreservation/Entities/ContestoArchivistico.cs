// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation.Entities
{
    public class ContestoArchivistico
    {
        public Classificazione[]? Classificazione { get; set; }

        public Fascicolazione[]? Fascicolazione { get; set; }

        public DocumentoCollegato? DocumentoCollegato { get; set; }
    }

    public class Classificazione
    {
        public string? CodiceClassificazione { get; set; }

        public string? TitolarioDiRiferimento { get; set; }
    }

    public class Fascicolazione
    {
        public string? CodiceFascicolo { get; set; }

        public string? DescrizioneFascicolo { get; set; }

        public string? CodiceSottoFascicolo { get; set; }

        public string? DescrizioneSottoFascicolo { get; set; }

        public string? TitolarioDiRiferimento { get; set; }
    }

    public class DocumentoCollegato
    {
        public string? IdDocumento { get; set; }

        public string? DataCreazione { get; set; }

        public string? Oggetto { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? NumeroProtocollo { get; set; }

        public string? DataProtocollo { get; set; }
    }
}
