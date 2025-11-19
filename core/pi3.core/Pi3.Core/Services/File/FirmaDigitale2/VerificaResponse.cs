// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale2
{
    public class VerificaResponse
    {
        public Documento? Documento { get; set; } = null;

        public EsitoVerificaFirma? Esito { get; set; } = null;

        public EsitoWarningVerificaFirma? Warning { get; set; } = null;
    }

    public class EsitoVerificaFirma
    {
        public DateTime DataVerificaFirma { get; set; }

        public bool FileMarcato { get; set; }

        public List<DatiFirmatari> DatiFirmatari { get; set; } = null!;

        public string DatiGeneraliVerifica { get; set; } = null!;

        public MarcaDetached MarcaDetached { get; set; } = null!;

        public DatiFirma ParteFirmata { get; set; } = null!;
    }

    public class DatiFirmatari
    {
        public Firmatario Firmatario { get; set; } = null!;
        public MarcaDetached MarcaFirma { get; set; } = null!;

    }

    public class Firmatario
    {
        public string DistinguishName { get; set; } = null!;
        public string CommonName { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;
        public string Organizzazione { get; set; } = null!;
        public string CnCertAuthority { get; set; } = null!;
        public string Nazione { get; set; } = null!;
        public DateTime DataOraFirma { get; set; }
        public string DigestAlgorithm { get; set; } = null!;
        public DateTime DataInizioValiditaCert { get; set; }
        public DateTime DataFineValiditaCert { get; set; }
        public string Cognome { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string CodiceFiscale { get; set; } = null!;
    }

    public class Documento
    {
        public string MediaType { get; set; } = null!;

        public byte[] FileOriginale { get; set; } = null!;
    }

    public class MarcaDetached
    {
        public string TSserialNumber { get; set; } = null!;
        public string TSANameSubject { get; set; } = null!;
        public DateTime DataInizioValiditaCert { get; set; }
        public string TSANameIssuer { get; set; } = null!;
        public string TSimprint { get; set; } = null!;
        public DateTime DataFineValiditaCert { get; set; }
        public DateTime TSdateTime { get; set; }
    }

    public class DatiFirma
    {
        public List<DatiFirmatari> DatiFirmatari { get; set; } = null!;

        public DatiFirma ParteFirmata { get; set; } = null!;
    }


    public class EsitoWarningVerificaFirma
    {
        public WarningFault[] WarningFault { get; set; } = null!;

        public WarningDettaglioFirmaDigitale DettaglioFirmaDigitale { get; set; } = null!;
    }

    public class WarningFault
    {
        public string SubjectCN { get; set; } = null!;

        public string SubjectDN { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string ErrorCode { get; set; } = null!;

        public string ErrorMsg { get; set; } = null!;
    }

    public class WarningDettaglioFirmaDigitale
    {
        public bool FileMarcato { get; set; }

        public List<DatiFirmatari> DatiFirmatari { get; set; } = null!;

        public string DatiGeneraliVerifica { get; set; } = null!;
    }
}
