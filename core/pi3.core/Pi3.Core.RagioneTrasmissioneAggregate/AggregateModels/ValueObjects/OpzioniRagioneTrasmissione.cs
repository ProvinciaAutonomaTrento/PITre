// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.ValueObjects
{
    public class OpzioniRagioneTrasmissione : ValueObject
    {
        public OpzioniRagioneTrasmissione()
        { }

        public bool RagioneDiSistema { get; init; }

        public TipoRagioneTrasmissioneEnum TipoRagione { get; init; } = TipoRagioneTrasmissioneEnum.SenzaWorkflow;

        public TipiDirittiTrasmissioneEnum TipoDirittoDestinatari { get; init; } = TipiDirittiTrasmissioneEnum.Nessuno;

        public bool Visible { get; init; }

        public bool PrevedeRisposta { get; init; }

        public bool Risposta { get; init; }

        public bool EstendiSuperioriGerarchici { get; init; }

        public TipiDestinatariTrasmissioneEnum TipoDestinatario { get; init; } = TipiDestinatariTrasmissioneEnum.Tutti;

        public TipiCessioneDirittiEnum TipoCessioneDiritto { get; init; } = TipiCessioneDirittiEnum.No;

        public TipiDirittiTrasmissioneEnum MantieneDiritti { get; init; } = TipiDirittiTrasmissioneEnum.Nessuno;

        public OpzioniNotificaTrasmissionePerEmail? NotificaTramissionePerEmail { get; init; } = null;

        public OpzioniMessaggioNotificaTrasmissione? MessaggioNotificaTrasmissione { get; init; } = null;
    }
}
