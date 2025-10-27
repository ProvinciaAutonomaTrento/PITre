// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Events
{
    public class AggregazioneDocumentaleCreatedEvent : ElementCreatedEvent
    {
        public AggregazioneDocumentaleCreatedEvent() : base()
        {
        }

        public TipiAggregazioneEnum TipoAggregazione { get; init; }

        public TipologieFascicoloEnum? TipologiaFascicolo { get; init; }

        public TipologieVisibilitaEnum? TipologiaVisibilita { get; init; }
    }

    public class SerieDocumentaleAssignedEvent : Event
    {
        public SerieDocumentaleAssignedEvent()
        { }

        public string IdSerieDocumentale { get; init; }

        public TextValue? Denominazione { get; init; }
    }

    public class TipologiaFascicoloAssignedEvent : Event
    {
        public TipologiaFascicoloAssignedEvent()
        { }

        public TipologieFascicoloEnum NewTipologiaFascicolo { get; init; }
    }


    public class TipologiaVisibilitaEvent : Event
    {
        public TipologiaVisibilitaEvent()
        { }

        public TipologieVisibilitaEnum NewTipologiaVisibilita { get; init; }
    }


    public class AmministrazioneTitolareAssignedEvent : Event
    {
        public AmministrazioneTitolareAssignedEvent()
        { }

        public AmministrazioneTitolare? AmministrazioneTitolare { get; init; }
    }

    public class AmministrazionePartecipanteAssignedEvent : Event
    {
        public AmministrazionePartecipanteAssignedEvent()
        { }

        public AmministrazionePartecipante? AmministrazionePartecipante { get; init; }
    }

    public class SoggettoIntestatarioPersonaGiuridicaAssignedEvent : Event
    {
        public SoggettoIntestatarioPersonaGiuridicaAssignedEvent()
        { }

        public SoggettoIntestatarioPersonaGiuridica? SoggettoIntestatarioPersonaGiuridica { get; init; }
    }

    public class SoggettoIntestatarioPersonaFisicaAssignedEvent : Event
    {
        public SoggettoIntestatarioPersonaFisicaAssignedEvent()
        { }

        public SoggettoIntestatarioPersonaFisica? SoggettoIntestatarioPersonaFisica { get; init; }
    }

    public class RUPAssignedEvent : Event
    {
        public RUPAssignedEvent()
        { }

        public RUP? RUP { get; init; }
    }

    public class AssegnatarioAssignedEvent : Event
    {
        public AssegnatarioAssignedEvent()
        { }

        public Assegnatario? Assegnatario { get; init; }
    }

    public class AssegnazioneAddedEvent : Event
    {
        public AssegnazioneAddedEvent()
        { }

        public Assegnazione Assegnazione { get; init; }
    }

    public class ApertoEvent : Event
    {
        public ApertoEvent()
        { }

        public DateTime? NewDataApertura { get; init; }
    }

    public class ChiusoEvent : Event
    {
        public ChiusoEvent()
        { }

        public DateTime? NewDataChiusura { get; init; }
    }

    public class ProgressivoAssignedEvent : Event
    {
        public ProgressivoAssignedEvent()
        { }

        public int Progressivo { get; init; }
    }

    public class ProcedimentoAmministrativoAssignedEvent : Event
    {
        public ProcedimentoAmministrativoAssignedEvent()
        { }


        public ProcedimentoAmministrativo ProcedimentoAmministrativo { get; init; }
    }

    public class IdDocAddedEvent : Event
    {
        public IdDocAddedEvent()
        { }

        public IdDoc IdDoc { get; init; }

        public string? IdFolder { get; init; }
    }

    public class IdDocRemovedEvent : Event
    {
        public IdDocRemovedEvent()
        { }

        public IdDoc IdDoc { get; init; }

        public string? IdFolder { get; init; }
    }

    public class IdAggPrimarioAssignedEvent : Event
    {
        public IdAggPrimarioAssignedEvent()
        { }

        public IdAgg? NewIdAggPrimario { get; init; }
    }

    public class TempoDiConservazioneChangedEvent : Event
    {
        public TempoDiConservazioneChangedEvent()
        { }

        public int? TempoDiConservazione { get; init; }
    }

    public class NotaAddedEvent : Event
    {
        public NotaAddedEvent()
        { }

        public string IdNota { get; init; }

        public TipologiaVisibilitaNotaEnum TipologiaVisibilita { get; init; }

        public TextValue Testo { get; init; }
    }

    public class FolderHierarchyCreatedEvent : Event
    {
        public FolderHierarchyCreatedEvent()
        { }

        public FolderHierarcy FolderHierarcy { get; init; }
    }

    public class DatiRegistrazioneAssignedEvent : Event
    {
        public DatiRegistrazioneAssignedEvent()
        { }

        public DatiRegistrazione DatiRegistrazione { get; init; }
    }

    public class CollocazioneFisicaAssignedEvent : Event
    {
        public CollocazioneFisicaAssignedEvent()
        { }

        public CollocazioneFisica CollocazioneFisica { get; init; }
    }

    public class FolderAddedEvent : Event
    {
        public FolderAddedEvent() : base()
        {
        }

        public string IdFolder { get; init; }

        public TextValue Name { get; init; }

        public IReadOnlyList<IdDoc>? IdDocs { get; init; }

        public string? IdParent { get; init; }
    }

    public class FolderRemovedEvent : Event
    {
        public FolderRemovedEvent() : base()
        {
        }

        public string IdFolder { get; init; }
    }

    public class DataScadenzaAssignedEvent : Event
    {
        public DataScadenzaAssignedEvent()
        { }

        public DateTime? NewDataScadenza { get; init; }
    }
}
