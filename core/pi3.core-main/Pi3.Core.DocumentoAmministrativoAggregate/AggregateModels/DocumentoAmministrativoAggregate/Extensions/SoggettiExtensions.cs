// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Metadati;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Segnatura;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Extensions
{
    internal static class AmministrazioneExtensions
    {
        public static CodiceIPAType AsCodiceIPA(this Amministrazione amministrazione)
        {
            amministrazione = amministrazione ?? throw new ArgumentNullException(nameof(amministrazione));

            return new CodiceIPAType()
            {
                CodiceIPA = amministrazione.CodiceIPA,
                Denominazione = amministrazione.Denominazione?.ToString()
            };
        }
    }

    internal static class PGExtensions
    {
        public static PGType AsPGType(this PG pG)
        {
            pG = pG ?? throw new ArgumentNullException(nameof(pG));

            return new PGType()
            {
                IndirizziDigitaliDiRiferimento = pG.IndirizziDigitaliDiRiferimento?.ToArray(),
                DenominazioneOrganizzazione = pG.DenominazioneOrganizzazione?.ToString(),
                DenominazioneUfficio = pG.DenominazioneUfficio?.ToString(),
                CodiceFiscale_PartitaIva = pG.CodiceFiscalePartitaIva
            };
        }
    }

    internal static class PFExtensions
    {
        public static PFType AsPFType(this PF pF)
        {
            pF = pF ?? throw new ArgumentNullException(nameof(pF));

            return new PFType()
            {
                IndirizziDigitaliDiRiferimento = pF.IndirizziDigitaliDiRiferimento?.ToArray(),
                CodiceFiscale = pF.CodiceFiscale,
                Cognome = pF.Cognome,
                Nome = pF.Nome,
                IPAAOO = (pF.AOO != null! ? pF.AOO.AsCodiceIPA() : null),
                IPAUOR = (pF.UOR != null! ? pF.UOR.AsCodiceIPA() : null)
            };
        }
    }

    internal static class PAIExtensions
    {
        public static PAIType AsPAIType(this PAI pAI)
        {
            pAI = pAI ?? throw new ArgumentNullException(nameof(pAI));
            
            return new PAIType()
            {
                IndirizziDigitaliDiRiferimento = pAI.IndirizziDigitaliDiRiferimento?.ToArray(),
                IPAAmm = (pAI.Amministrazione != null! ? pAI.Amministrazione.AsCodiceIPA() : null),
                IPAAOO = (pAI.AOO!= null! ? pAI.AOO.AsCodiceIPA() : null),
                IPAUOR = (pAI.UOR != null! ? pAI.UOR.AsCodiceIPA() : null)
            };
        }
    }

    internal static class PAEExtensions
    {
        public static PAEType AsPAEType(this PAE pAE)
        {
            pAE = pAE ?? throw new ArgumentNullException(nameof(pAE));

            return new PAEType()
            {
                IndirizziDigitaliDiRiferimento = pAE.IndirizziDigitaliDiRiferimento?.ToArray(),
                DenominazioneAmministrazione = pAE.DenominazioneAmministrazione?.ToString(),
                DenominazioneUfficio = pAE.DenominazioneUfficio?.ToString()
            };
        }
    }

    internal static class AmministrazioneCheEffettuaLaRegistrazioneExtensions
    {
        public static TipoSoggetto1Type AsTipoSoggetto1Type(this AmministrazioneCheEffettuaLaRegistrazione amministrazioneCheEffettuaLaRegistrazione)
        {
            amministrazioneCheEffettuaLaRegistrazione = amministrazioneCheEffettuaLaRegistrazione ?? throw new ArgumentNullException(nameof(amministrazioneCheEffettuaLaRegistrazione));

            return new TipoSoggetto1Type()
            {
                PAI = amministrazioneCheEffettuaLaRegistrazione.PAI.AsPAIType()
            };
        }
    }

    internal static class AutoreExtensions
    {
        public static TipoSoggetto41Type AsTipoSoggetto41Type(this Autore autore)
        {
            autore = autore ?? throw new ArgumentNullException(nameof(autore));
            
            object item = null!;

            if (autore.PAI != null!)
                item = autore.PAI?.AsPAIType()!;
            else if (autore.PAE != null!)
                item = autore.PAE?.AsPAEType()!;
            else if (autore.PG != null!)
                item = autore.PG?.AsPGType()!;
            else if (autore.PF != null!)
                item = autore.PF?.AsPFType()!;

            return new TipoSoggetto41Type()
            {
                Item = item
            };
        }
    }

    internal static class MittenteExtensions
    {
        public static TipoSoggetto32Type AsTipoSoggetto32Type(this Mittente mittente)
        {
            mittente = mittente ?? throw new ArgumentNullException(nameof(mittente));

            object item = null!;

            if (mittente.PAI != null!)
                item = mittente.PAI?.AsPAIType()!;
            else if (mittente.PAE != null!)
                item = mittente.PAE?.AsPAEType()!;
            else if (mittente.PG != null!)
                item = mittente.PG?.AsPGType()!;
            else if (mittente.PF != null!)
                item = mittente.PF?.AsPFType()!;

            return new TipoSoggetto32Type()
            {
                Item = item
            };
        }
    }

    internal static class DestinatarioExtensions
    {
        public static TipoSoggetto31Type AsTipoSoggetto31Type(this Destinatario destinatario)
        {
            destinatario = destinatario ?? throw new ArgumentNullException(nameof(destinatario));

            object item = null!;

            if (destinatario.PAI != null!)
                item = destinatario.PAI?.AsPAIType()!;
            else if (destinatario.PAE != null!)
                item = destinatario.PAE?.AsPAEType()!;
            else if (destinatario.PG != null!)
                item = destinatario.PG?.AsPGType()!;
            else if (destinatario.PF != null!)
                item = destinatario.PF?.AsPFType()!;

            return new TipoSoggetto31Type()
            {
                Item = item
            };
        }
    }

    internal static class OperatoreExtensions
    {
        public static TipoSoggetto42Type AsTipoSoggetto42Type(this Operatore operatore)
        {
            operatore = operatore ?? throw new ArgumentNullException(nameof(operatore));

            return new TipoSoggetto42Type()
            {
                PF = operatore.PF?.AsPFType()
            };
        }
    }

    internal static class ResponsabileGestioneDocumentaleExtensions
    {
        public static TipoSoggetto43Type AsTipoSoggetto43Type(this ResponsabileGestioneDocumentale responsabileGestioneDocumentale)
        {
            responsabileGestioneDocumentale = responsabileGestioneDocumentale ?? throw new ArgumentNullException(nameof(responsabileGestioneDocumentale));

            return new TipoSoggetto43Type()
            {
                PF = responsabileGestioneDocumentale.PF?.AsPFType()
            };
        }
    }

    internal static class ResponsabileServizioProtocolloExtensions
    {
        public static TipoSoggetto44Type AsTipoSoggetto44Type(this ResponsabileServizioProtocollo responsabileServizioProtocollo)
        {
            responsabileServizioProtocollo = responsabileServizioProtocollo ?? throw new ArgumentNullException(nameof(responsabileServizioProtocollo));

            return new TipoSoggetto44Type()
            {
                PF = responsabileServizioProtocollo.PF?.AsPFType()
            };
        }
    }

    internal static class AssegnatarioExtensions
    {
        public static TipoSoggetto2Type AsTipoSoggetto2Type(this Assegnatario assegnatario)
        {
            assegnatario = assegnatario ?? throw new ArgumentNullException(nameof(assegnatario));

            return new TipoSoggetto2Type()
            {
                AS = new ASType()
                {
                    CodiceFiscale = assegnatario.CodiceFiscale,
                    IndirizziDigitaliDiRiferimento = assegnatario.IndirizziDigitaliDiRiferimento?.ToArray(),
                    Cognome = assegnatario.Cognome,
                    Nome = assegnatario.Nome,
                    IPAAmm = assegnatario.Amministrazione?.AsCodiceIPA(),
                    IPAAOO = assegnatario.AOO?.AsCodiceIPA(),
                    IPAUOR = assegnatario.UOR?.AsCodiceIPA()
                }
            };
        }
    }

    internal static class RUPExtensions
    {
        public static TipoSoggetto6Type AsTipoSoggetto6Type(this RUP rUP)
        {
            rUP = rUP ?? throw new ArgumentNullException(nameof(rUP));

            return new TipoSoggetto6Type()
            {
                RUP = new RUPType()
                {
                    CodiceFiscale = rUP.CodiceFiscale,
                    IndirizziDigitaliDiRiferimento = rUP.IndirizziDigitaliDiRiferimento?.ToArray(),
                    Cognome = rUP.Cognome,
                    Nome = rUP.Nome,
                    IPAAmm = rUP.Amministrazione?.AsCodiceIPA(),
                    IPAAOO = rUP.AOO?.AsCodiceIPA(),
                    IPAUOR = rUP.UOR?.AsCodiceIPA()
                }
            };
        }
    }

    internal static class SWProduttoreExtensions
    {
        public static TipoSoggetto5Type AsTipoSoggetto5Type(this SwProduttore swProduttore)
        {
            swProduttore = swProduttore ?? throw new ArgumentNullException(nameof(swProduttore));

            return new TipoSoggetto5Type()
            {
                SW = new SWType()
                {
                    DenominazioneSistema = swProduttore.Value
                }
            };
        }
    }
}
