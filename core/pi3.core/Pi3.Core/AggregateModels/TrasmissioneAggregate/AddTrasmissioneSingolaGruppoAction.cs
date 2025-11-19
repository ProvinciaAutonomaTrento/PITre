// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{
    public class DatiUtenteNotificatoTrasmissioneSingolaGruppo : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string IdUtente { get; init; }

        public string? UserId { get; init; } = null;

        public string? Cognome { get; init; } = null;

        public string? Nome { get; init; } = null;
    }

    public class DatiTrasmissioneSingolaGruppo : ValueObject
    {
        public DatiTrasmissioneSingolaGruppo()
        { }
        
        [Required(AllowEmptyStrings = false)]
        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; } = null;

        public bool? RagioneConWorkflow { get; init; } = false;

        public CessioneDirittiRagione? CessioneDirittiRagione { get; init; } = null;

        [Required(AllowEmptyStrings = false)]
        public string IdGruppoDestinatario { get; init; }

        public string? CodiceGruppoDestinatario { get; init; } = null;

        public TextValue? DescrizioneGruppoDestinatario { get; init; } = null;

        public TipiTrasmissioneSingolaEnum Tipo { get; init; }

        public TextValue? Note { get; init; } = null;

        public DateTime? DataScadenza { get; init; } = null;

        public bool NascondiVersioniPrecedenti { get; init; }

        public List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>? UtentiNotificati { get; init; } = null;
    }

    public class DatiTrasmissioneSingolaUtente : ValueObject
    {
        public DatiTrasmissioneSingolaUtente()
        { }

        [Required(AllowEmptyStrings = false)]
        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; } = null;

        public bool? RagioneConWorkflow { get; init; } = false;

        public CessioneDirittiRagione? CessioneDirittiRagione { get; init; } = null;

        [Required(AllowEmptyStrings = false)]
        public string IdUtente { get; init; }

        public string? UserId { get; init; } = null;

        public string? Cognome { get; init; } = null;

        public string? Nome { get; init; } = null;

        public TextValue? Note { get; init; } = null;

        public DateTime? DataScadenza { get; init; } = null;

        public bool NascondiVersioniPrecedenti { get; init; }
    }
}