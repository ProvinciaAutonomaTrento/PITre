// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json.Linq;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate
{
    public enum TipoRagioneTrasmissioneEnum
    {
        SenzaWorkflow,
        ConWorkflow,
        Interoperabilita
    }

    public enum TipiDirittiTrasmissioneEnum
    {
        Nessuno,
        Scrittura,
        Lettura 
    }

    public enum TipiDestinatariTrasmissioneEnum
    {
        Tutti,
        SoloSuperiori,
        SoloSottoposti,
        PariLivello
    }

    public enum TipiCessioneDirittiEnum
    {
        No,
        Si,
        SiConSceltaUtente
    }

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

    public class OpzioniMessaggioNotificaTrasmissione : ValueObject
    {
        public OpzioniMessaggioNotificaTrasmissione()
        { }

        public string TestoPerNotificaDocumenti { get; init; }
        public string TestoPerNotificaFascicoli { get; init; }

    }

    public class OpzioniNotificaTrasmissionePerEmail : ValueObject
    {
        public OpzioniNotificaTrasmissionePerEmail()
        { }

        public bool IncludiLinkSchedaDettaglio { get; init; }
        public bool IncludiLinkImmagineDocumento { get; init; }
        public bool AllegaDocumenti { get; init; }

    }

}
