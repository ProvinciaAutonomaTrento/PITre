// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
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

    public class AmministrazioneCheEffettuaLaRegistrazione : Soggetto
    {
        public AmministrazioneCheEffettuaLaRegistrazione() : base()
        {
        }

        public AmministrazioneCheEffettuaLaRegistrazione(string? id = null) : base(id)
        {
        }

        public AmministrazioneCheEffettuaLaRegistrazione(PAI pAI, string? id = null)
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

    public class Autore : Soggetto
    {
        protected string _toString;

        public Autore() : base()
        {
        }

        public Autore(string? id = null) : base(id)
        {
        }

        public Autore(PAI pai, string? id = null)
        {
            PAI = pai;
            _toString = pai.ToString();
            Id = id;
        }

        public Autore(PF pf, string? id = null)
        {
            PF = pf;
            _toString = pf.ToString();
            Id = id;
        }

        public Autore(PG pg, string? id = null)
        {
            PG = pg;
            _toString = pg.ToString();
            Id = id;
        }

        public Autore(PAE pae, string? id = null)
        {
            PAE = pae;
            _toString = pae.ToString();
            Id = id;
        }

        public PAI PAI { get; init; }

        public PF PF { get; init; }

        public PG PG { get; init; }

        public PAE PAE { get; init; }

        public override string ToString()
        {
            return this._toString;
        }
    }

    public class Mittente : Soggetto
    {
        protected string _toString;

        public Mittente() : base()
        {
        }

        public Mittente(string? id = null) : base(id)
        {
        }

        public Mittente(PAI pai, string? id = null)
        {
            PAI = pai;
            _toString = pai.ToString();
            Id = id;
        }

        public Mittente(PF pf, string? id = null)
        {
            PF = pf;
            _toString = pf.ToString();
            Id = id;
        }

        public Mittente(PG pg, string? id = null)
        {
            PG = pg;
            _toString = pg.ToString();
            Id = id;
        }

        public Mittente(PAE pae, string? id = null)
        {
            PAE = pae;
            _toString = pae.ToString();
            Id = id;
        }

        public PAI PAI { get; init; }

        public PF PF { get; init; }

        public PG PG { get; init; }

        public PAE PAE { get; init; }

        public override string ToString()
        {
            return this._toString;
        }
    }

    public class Destinatario : Soggetto
    {
        protected string _toString;

        public Destinatario() : base()
        {
        }

        public Destinatario(string? id = null) : base(id)
        {
        }

        public Destinatario(PAI pai, string? id = null)
        {
            PAI = pai;
            _toString = pai.ToString();
            Id = id;
        }

        public Destinatario(PF pf, string? id = null)
        {
            PF = pf;
            _toString = pf.ToString();
            Id = id;
        }

        public Destinatario(PG pg, string? id = null)
        {
            PG = pg;
            _toString = pg.ToString();
            Id = id;
        }

        public Destinatario(PAE pae, string? id = null)
        {
            PAE = pae;
            _toString = pae.ToString();
            Id = id;
        }

        public PAI PAI { get; init; }

        public PF PF { get; init; }

        public PG PG { get; init; }

        public PAE PAE { get; init; }

        public override string ToString()
        {
            return this._toString;
        }
    }

    public class Operatore : Soggetto
    {
        public Operatore() : base()
        {
        }

        public Operatore(string? id = null) : base(id)
        {
        }

        public Operatore(PF pF, string? id = null)
        {
            PF = pF;
            Id = id;
        }

        public PF PF { get; init; }

        public override string ToString()
        {
            return this.PF?.ToString() ?? String.Empty;
        }
    }

    public class ResponsabileGestioneDocumentale : Soggetto
    {
        public ResponsabileGestioneDocumentale() : base()
        {
        }

        public ResponsabileGestioneDocumentale(string? id = null) : base(id)
        {
        }

        public ResponsabileGestioneDocumentale(PF pF, string? id = null)
        {
            PF = pF;
            Id = id;
        }

        public PF PF { get; init; }

        public override string ToString()
        {
            return this.PF?.ToString() ?? String.Empty;
        }
    }

    public class ResponsabileServizioProtocollo : Soggetto
    {
        public ResponsabileServizioProtocollo() : base()
        {
        }

        public ResponsabileServizioProtocollo(string? id = null) : base(id)
        {
        }

        public ResponsabileServizioProtocollo(PF pF, string? id = null)
        {
            PF = pF;
            Id = id;
        }

        public PF PF { get; init; }

        public override string ToString()
        {
            return this.PF?.ToString() ?? String.Empty;
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

    public class SwProduttore : Soggetto
    {
        public SwProduttore() : base()
        {
        }

        public SwProduttore(string? id = null) : base(id)
        {
        }

        [Required]
        public string Value { get; init; }

        public override string ToString()
        {
            return this.Value;
        }
    }
}