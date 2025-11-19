// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public class Amministrazione : ValueObject
    {
        public Amministrazione() : base()
        {
        }

        public string CodiceIPA { get; init; }

        [Required()]
        public TextValue Denominazione { get; init; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.CodiceIPA))
                return $"{this.Denominazione} - {this.CodiceIPA}";
            else
                return $"{this.Denominazione}";
        }
    }

    public class PF : ValueObject
    {
        public PF() : base()
        {
        }

        [Required()]
        public string Cognome { get; init; }

        [Required()]
        public string Nome { get; init; }

        [RegularExpression("^[a-zA-Z]{6}[0-9]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9]{2}([a-zA-Z]{1}[0-9]{3})[a-zA-Z]{1}$")]
        public string CodiceFiscale { get; init; }

        public Amministrazione Amministrazione { get; init; }

        public Amministrazione AOO { get; init; }

        public Amministrazione UOR { get; init; }

        public List<string> IndirizziDigitaliDiRiferimento { get; init; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.CodiceFiscale))
                return $"{this.Cognome} {this.Nome} {this.CodiceFiscale}";
            else
                return $"{this.Cognome} {this.Nome}";
        }
    }

    public class PAE : ValueObject
    {
        public PAE() : base()
        {
        }

        [Required()]
        public TextValue DenominazioneAmministrazione { get; init; }

        public TextValue DenominazioneUfficio { get; init; }

        public List<string> IndirizziDigitaliDiRiferimento { get; init; }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(this.DenominazioneUfficio.ToString()) ? this.DenominazioneUfficio.ToString() : this.DenominazioneAmministrazione.ToString();
        }
    }

    public class PG : ValueObject
    {
        public PG() : base()
        {
        }

        [Required()]
        public TextValue DenominazioneOrganizzazione { get; init; }

        [RegularExpression("^[a-zA-Z]{6}[0-9]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9]{2}([a-zA-Z]{1}[0-9]{3})[a-zA-Z]{1}$")]
        public string CodiceFiscalePartitaIva { get; init; }

        public TextValue DenominazioneUfficio { get; init; }

        public List<string> IndirizziDigitaliDiRiferimento { get; init; }

        public override string ToString()
        {
            return (this.DenominazioneUfficio != null ? this.DenominazioneUfficio.ToString() : this.DenominazioneOrganizzazione.ToString());
        }
    }

    public class PAI : ValueObject
    {
        public PAI() : base()
        {
        }

        [Required()]
        public Amministrazione Amministrazione { get; init; }

        [Required()]
        public Amministrazione AOO { get; init; }

        public Amministrazione UOR { get; init; }

        [Required()]
        public List<string> IndirizziDigitaliDiRiferimento { get; init; }

        public override string ToString()
        {
            if (this.UOR != null)
                return this.UOR.ToString();
            else
                return this.AOO.ToString();
        }
    }

    public abstract class Soggetto : ValueObject
    {
        public Soggetto() : base()
        {
        }

        public Soggetto(string? id = null) : base()
        {
            this.Id = id;
        }

        public virtual string Ruolo
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public string? Id { get; set; } = null;

        public abstract string ToString();
    }

    public class AmministrazioneTitolare : Soggetto
    {
        public AmministrazioneTitolare() : base()
        {
        }

        public AmministrazioneTitolare(string? id = null) : base(id)
        {
        }

        public AmministrazioneTitolare(PAI pAI, string? id = null)
        {
            PAI = pAI;
            Id = id;
        }

        public PAI PAI { get; init; }

        public override string ToString()
        {
            return this.PAI?.ToString() ?? String.Empty;
        }
    }

    public class AmministrazionePartecipante : Soggetto
    {
        protected string _toString;

        public AmministrazionePartecipante() : base()
        {
        }

        public AmministrazionePartecipante(string? id = null) : base(id)
        {
        }

        public AmministrazionePartecipante(PAI pAI, string? id = null)
        {
            PAI = pAI;
            _toString = pAI.ToString();
            Id = id;
        }

        public AmministrazionePartecipante(PAE pAE, string? id = null)
        {
            PAE = pAE;
            _toString = pAE.ToString();
            Id = id;
        }

        public PAI PAI { get; init; }

        public PAE PAE { get; init; }

        public override string ToString()
        {
            return _toString;
        }
    }

    public class SoggettoIntestatarioPersonaGiuridica : Soggetto
    {
        protected string _toString;

        public SoggettoIntestatarioPersonaGiuridica() : base()
        {
        }

        public SoggettoIntestatarioPersonaGiuridica(string? id = null) : base(id)
        {
        }

        public SoggettoIntestatarioPersonaGiuridica(PG pG, string? id = null)
        {
            PG = pG;
            _toString = pG.ToString();
            Id = id;
        }

        public SoggettoIntestatarioPersonaGiuridica(PAI pAI, string? id = null)
        {
            PAI = pAI;
            _toString = pAI.ToString();
            Id = id;
        }

        public SoggettoIntestatarioPersonaGiuridica(PAE pAE, string? id = null)
        {
            PAE = pAE;
            _toString = pAE.ToString();
            Id = id;
        }

        public PG PG { get; init; }

        public PAI PAI { get; init; }

        public PAE PAE { get; init; }

        public override string ToString()
        {
            return _toString;
        }
    }

    public class SoggettoIntestatarioPersonaFisica : Soggetto
    {
        protected string _toString;

        public SoggettoIntestatarioPersonaFisica() : base()
        {
        }

        public SoggettoIntestatarioPersonaFisica(string? id = null) : base(id)
        {
        }

        public SoggettoIntestatarioPersonaFisica(PF pF, string? id = null)
        {
            PF = pF;
            _toString = pF.ToString();
            Id = id;
        }

        public PF PF { get; init; }

        public override string ToString()
        {
            return _toString;
        }
    }

    public class RUP : Soggetto
    {
        public RUP() : base()
        {
        }

        public RUP(string? id = null) : base(id)
        {
        }

        [Required]
        public string Cognome { get; init; }

        [Required]
        public string Nome { get; init; }

        [RegularExpression("^[a-zA-Z]{6}[0-9]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9]{2}([a-zA-Z]{1}[0-9]{3})[a-zA-Z]{1}$")]
        public string CodiceFiscale { get; init; }

        [Required()]
        public Amministrazione Amministrazione { get; init; }

        [Required()]
        public Amministrazione AOO { get; init; }

        [Required()]
        public Amministrazione UOR { get; init; }

        [Required()]
        public List<string> IndirizziDigitaliDiRiferimento { get; init; }

        public override string ToString()
        {
            return $"{this.Cognome} {this.Nome} ({this.UOR.ToString()})";
        }
    }

    public class Assegnatario : Soggetto
    {
        public Assegnatario() : base()
        {
        }

        public Assegnatario(string? id = null) : base(id)
        {
        }

        public string Cognome { get; init; }

        public string Nome { get; init; }

        public string CodiceFiscale { get; init; }

        [Required()]
        public Amministrazione Amministrazione { get; init; }

        [Required()]
        public Amministrazione AOO { get; init; }

        [Required()]
        public Amministrazione UOR { get; init; }

        [Required()]
        public List<string> IndirizziDigitaliDiRiferimento { get; init; }

        public override string ToString()
        {
            return this.UOR.ToString();
        }
    }

}
