// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
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

        public string Id { get; init; }

        public string IdSerieDocumentale { get; init; }

        public TextValue? Denominazione { get; init; }
    }

    public class TipologiaFascicoloAssignedEvent : Event
    {
        public TipologiaFascicoloAssignedEvent()
        { }

        public string Id { get; init; }

        public TipologieFascicoloEnum NewTipologiaFascicolo { get; init; }
    }


    public class TipologiaVisibilitaEvent : Event
    {
        public TipologiaVisibilitaEvent()
        { }

        public string Id { get; init; }

        public TipologieVisibilitaEnum NewTipologiaVisibilita { get; init; }
    }


    public class AmministrazioneTitolareAssignedEvent : Event
    {
        public AmministrazioneTitolareAssignedEvent()
        { }

        public string Id { get; init; }

        public AmministrazioneTitolare? AmministrazioneTitolare { get; init; }
    }

    public class AmministrazionePartecipanteAssignedEvent : Event
    {
        public AmministrazionePartecipanteAssignedEvent()
        { }

        public string Id { get; init; }

        public AmministrazionePartecipante? AmministrazionePartecipante { get; init; }
    }

    public class SoggettoIntestatarioPersonaGiuridicaAssignedEvent : Event
    {
        public SoggettoIntestatarioPersonaGiuridicaAssignedEvent()
        { }

        public string Id { get; init; }

        public SoggettoIntestatarioPersonaGiuridica? SoggettoIntestatarioPersonaGiuridica { get; init; }
    }

    public class SoggettoIntestatarioPersonaFisicaAssignedEvent : Event
    {
        public SoggettoIntestatarioPersonaFisicaAssignedEvent()
        { }

        public string Id { get; init; }

        public SoggettoIntestatarioPersonaFisica? SoggettoIntestatarioPersonaFisica { get; init; }
    }

    public class RUPAssignedEvent : Event
    {
        public RUPAssignedEvent()
        { }

        public string Id { get; init; }

        public RUP? RUP { get; init; }
    }

    public class AssegnatarioAssignedEvent : Event
    {
        public AssegnatarioAssignedEvent()
        { }

        public string Id { get; init; }

        public Assegnatario? Assegnatario { get; init; }
    }

    public class AssegnazioneAddedEvent : Event
    {
        public AssegnazioneAddedEvent()
        { }

        public string Id { get; init; }

        public Assegnazione Assegnazione { get; init; }
    }

    public class ApertoEvent : Event
    {
        public ApertoEvent()
        { }

        public string Id { get; init; }

        public DateTime? NewDataApertura { get; init; }
    }

    public class ChiusoEvent : Event
    {
        public ChiusoEvent()
        { }

        public string Id { get; init; }

        public DateTime? NewDataChiusura { get; init; }
    }

    public class ProgressivoAssignedEvent : Event
    {
        public ProgressivoAssignedEvent()
        { }

        public string Id { get; init; }

        public int Progressivo { get; init; }
    }

    public class ProcedimentoAmministrativoAssignedEvent : Event
    {
        public ProcedimentoAmministrativoAssignedEvent()
        { }

        public string Id { get; init; }

        public ProcedimentoAmministrativo ProcedimentoAmministrativo { get; init; }
    }

    public class IdDocAddedEvent : Event
    {
        public IdDocAddedEvent()
        { }

        public string Id { get; init; }

        public IdDoc IdDoc { get; init; }

        public string? IdFolder { get; init; }
    }

    public class IdDocRemovedEvent : Event
    {
        public IdDocRemovedEvent()
        { }

        public string Id { get; init; }

        public IdDoc IdDoc { get; init; }

        public string? IdFolder { get; init; }
    }

    public class PosizioneFisicaAggregazioneDocumentaleAssignedEvent : Event
    {
        public PosizioneFisicaAggregazioneDocumentaleAssignedEvent()
        { }

        public string Id { get; init; }

        public string? NewPosizioneFisicaAggregazioneDocumentale { get; init; }
    }

    public class IdAggPrimarioAssignedEvent : Event
    {
        public IdAggPrimarioAssignedEvent()
        { }

        public string Id { get; init; }

        public IdAgg? NewIdAggPrimario { get; init; }
    }

    public class TempoDiConservazioneChangedEvent : Event
    {
        public TempoDiConservazioneChangedEvent()
        { }

        public string Id { get; init; }

        public int? TempoDiConservazione { get; init; }
    }

    public class NotaAddedEvent : Event
    {
        public NotaAddedEvent()
        { }

        public string Id { get; init; }

        public string IdNota { get; init; }

        public TipologiaVisibilitaNotaEnum TipologiaVisibilita { get; init; }
        
        public TextValue Testo { get; init; }
    }

    public class FolderHierarchyCreatedEvent : Event
    {
        public FolderHierarchyCreatedEvent()
        { }

        public string Id { get; init; }

        public FolderHierarcy FolderHierarcy { get; init; }
    }

    public class DatiRegistrazioneAssignedEvent : Event
    {
        public DatiRegistrazioneAssignedEvent()
        { }

        public string Id { get; init; }

        public DatiRegistrazione DatiRegistrazione { get; init; }
    }


    public class RegistrazioneRichiestaEvent : Event
    {
        public RegistrazioneRichiestaEvent()
        { }

        public string Id { get; init; }

        public DatiRichiestaRegistrazione Registrazione { get; init; }
    }

    public class CollocazioneFisicaAssignedEvent : Event
    {
        public CollocazioneFisicaAssignedEvent()
        { }

        public string Id { get; init; }

        public CollocazioneFisica CollocazioneFisica { get; init; }
    }

    public class RegistroAssignedEvent : Event
    {
        public RegistroAssignedEvent() : base()
        {
        }
        public string Id { get; init; }

        public string IdRegistro { get; init; }

        public string? Description { get; init; }     

    }
}
