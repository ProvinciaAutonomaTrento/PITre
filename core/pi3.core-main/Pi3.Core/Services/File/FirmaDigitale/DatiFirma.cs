// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale
{
    public class DatiFirma : ValueObject
    {
        public string CnCertAuthority { get; init; } = null!;

        public string CodiceFiscale { get; init; } = null!;

        public string Cognome { get; init; } = null!;

        public string CommonName { get; init; } = null!;

        public DateTime? DataFineValiditaCert { get; init; } = null;

        public DateTime? DataInizioValiditaCert { get; init; } = null;

        public DateTime? DataOraFirma { get; init; } = null;

        public DateTime? DataRevocaCertificato { get; init; } = null;

        public string DigestAlgorithm { get; init; } = null!;

        public string DistinguishName { get; init; } = null!;

        public string Nazione { get; init; } = null!;

        public string Nome { get; init; } = null!;

        public string Organizzazione { get; init; } = null!;

        public string SerialNumber { get; init; } = null!;
    }
}
