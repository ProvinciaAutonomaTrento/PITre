// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects
{
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
}
