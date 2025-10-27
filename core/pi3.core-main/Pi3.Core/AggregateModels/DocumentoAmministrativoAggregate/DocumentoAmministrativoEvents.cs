// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Newtonsoft.Json;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public class DocumentoAmministrativoCreatedEvent : ElementCreatedEvent
    {
        public DocumentoAmministrativoCreatedEvent()
        { }

        public OggettoDelDocumento OggettoDelDocumento { get; init; }

        public TipologiaFlussoEnum? TipologiaFlusso { get; init; }

        public TipologieVisibilitaEnum? TipologiaVisibilita { get; init; }

        public IdDoc? IdDocPrimario { get; init; }
    }

    public class RegistrazioneRichiestaEvent : Event
    {
        public RegistrazioneRichiestaEvent()
        { }

        public DatiRichiestaRegistrazione Registrazione { get; init; }
    }

    public class OggettoDelDocumentoChanged : Event
    {
        public OggettoDelDocumentoChanged()
        { }

        public OggettoDelDocumento NewOggettoDelDocumento { get; init; }
    }

    public class IdDocAssignedEvent : Event
    {
        public IdDocAssignedEvent()
        { }

        public IdDoc IdDoc { get; init; }
    }

    public class DatiRegistrazioneProtocolloAssignedEvent : Event
    {
        public DatiRegistrazioneProtocolloAssignedEvent()
        { }

        public DatiRegistrazioneProtocollo DatiRegistrazione { get; init; }
    }

    public class DatiRegistrazioneRepertorioAssignedEvent : Event
    {
        public DatiRegistrazioneRepertorioAssignedEvent()
        { }

        public DatiRegistrazioneRepertorio DatiRegistrazione { get; init; }
    }

    public class TipologiaDocumentaleAssignedEvent : Event
    {
        public TipologiaDocumentaleAssignedEvent()
        { }

        public string? TipologiaDocumentale { get; init; }
    }

    public class AutoreAssignedEvent : Event
    {
        public AutoreAssignedEvent()
        { }

        public Autore Autore { get; init; }
    }

    public class MezzoSpedizioneAssignedEvent : Event
    {
        public MezzoSpedizioneAssignedEvent()
        { }

        public string IdMezzoSpedizione { get; init; }

        public TextValue? Descrizione { get; init; } = null;
    }

    public class MezzoSpedizioneRemovedEvent : Event
    {
        public MezzoSpedizioneRemovedEvent()
        { }
    }

    public class MittenteAssignedEvent : Event
    {
        public MittenteAssignedEvent()
        { }

        public Mittente Mittente { get; init; }
    }

    public class MittenteMultiploAddedEvent : Event
    {
        public MittenteMultiploAddedEvent()
        { }

        public Mittente Mittente { get; init; }
    }

    public class MittenteMultiploRemovedEvent : Event
    {
        public MittenteMultiploRemovedEvent()
        { }

        public Mittente Mittente { get; init; }
    }
    
    public class DestinatarioAddedEvent : Event
    {
        public DestinatarioAddedEvent()
        { }

        public Destinatario Destinatario { get; init; }
    }

    public class DestinatarioRemovedEvent : Event
    {
        public DestinatarioRemovedEvent()
        { }

        public Destinatario Destinatario { get; init; }
    }

    public class DestinatarioCcAddedEvent : Event
    {
        public DestinatarioCcAddedEvent()
        { }

        public Destinatario Destinatario { get; init; }
    }

    public class DestinatarioCcRemovedEvent : Event
    {
        public DestinatarioCcRemovedEvent()
        { }

        public Destinatario Destinatario { get; init; }
    }

    public class AssegnatarioAddedEvent : Event
    {
        public AssegnatarioAddedEvent()
        { }

        public Assegnatario Assegnatario { get; init; }
    }

    public class OperatoreAddedEvent : Event
    {
        public OperatoreAddedEvent()
        { }

        public Operatore Operatore { get; init; }
    }

    public class AmministrazioneAssignedEvent : Event
    {
        public AmministrazioneAssignedEvent()
        { }

        public AmministrazioneCheEffettuaLaRegistrazione? Amministrazione { get; init; } = null;
    }

    public class ResponsabileGestioneDocumentaleAssignedEvent : Event
    {
        public ResponsabileGestioneDocumentaleAssignedEvent()
        { }

        public ResponsabileGestioneDocumentale? ResponsabileGestioneDocumentale { get; init; } = null;
    }

    public class ResponsabileServizioProtocolloAssignedEvent : Event
    {
        public ResponsabileServizioProtocolloAssignedEvent()
        { }

        public ResponsabileServizioProtocollo? ResponsabileServizioProtocollo { get; init; } = null;
    }

    public class RUPAssignedEvent : Event
    {
        public RUPAssignedEvent()
        { }

        public RUP? RUP { get; init; } = null;
    }

    public class SwProduttoreAssignedEvent : Event
    {
        public SwProduttoreAssignedEvent()
        { }

        public SwProduttore? SwProduttore { get; init; } = null;
    }

    public class AllegatoAddedEvent : Event
    {
        public AllegatoAddedEvent()
        { }

        public Allegato Allegato { get; init; }
    }

    public class TipologiaVisibilitaChangedEvent : Event
    {
        public TipologiaVisibilitaChangedEvent()
        { }

        public TipologieVisibilitaEnum TipologiaVisibilita { get; init; }
    }

    public class AggFascicoloAddedEvent : Event
    {
        public AggFascicoloAddedEvent()
        { }

        public string IdFascicolo { get; init; }

        public TextValue? Denominazione { get; init; } = null;
    }

    public class AggFascicoloRemovedEvent : Event
    {
        public AggFascicoloRemovedEvent()
        { }

        public string IdFascicolo { get; init; }
    }

    public class AggSerieDocumentaleAddedEvent : Event
    {
        public AggSerieDocumentaleAddedEvent()
        { }

        public string IdSerieDocumentale { get; init; }

        public TextValue? Denominazione { get; init; } = null;
    }

    public class AggSerieDocumentaleRemovedEvent : Event
    {
        public AggSerieDocumentaleRemovedEvent()
        { }

        public string IdSerieDocumentale { get; init; }
    }

    public class AggSerieDiFascicoliAddedEvent : Event
    {
        public AggSerieDiFascicoliAddedEvent()
        { }

        public string IdSerieDiFascicoli { get; init; }

        public TextValue? Denominazione { get; init; } = null;
    }

    public class AggSerieDiFascicoliRemovedEvent : Event
    {
        public AggSerieDiFascicoliRemovedEvent()
        { }

        public string IdSerieDiFascicoli { get; init; }
    }

    public class TempoDiConservazioneChangedEvent : Event
    {
        public TempoDiConservazioneChangedEvent()
        { }

        public int? TempoDiConservazione { get; init; }
    }

    public class AnnullatoEvent : Event
    {
        public AnnullatoEvent()
        { }

        public Annullamento Annullamento { get; init; }
    }

    public class ProtocolloMittenteAssignedEvent : Event
    {
        public ProtocolloMittenteAssignedEvent()
        { }

        public ProtocolloMittente? ProtocolloMittente { get; init; }
    }

    public class ProtocolloEmergenzaAssignedEvent : Event
    {
        public ProtocolloEmergenzaAssignedEvent()
        { }

        public ProtocolloEmergenza? ProtocolloEmergenza { get; init; }
    }

    public class DocumentoConsolidatoEvent : Event
    {
        public DocumentoConsolidatoEvent()
        { }

        public Consolidamento Consolidamento { get; init; }
    }

    public class ClassificationOrAggAsPrincipaleAssigned : Event
    {
        public ClassificationOrAggAsPrincipaleAssigned()
        { }

        public IEntity<string>? ClassificationOrAggPrincipale { get; init; }
    }

    public class NotaAddedEvent : Event
    {
        public NotaAddedEvent()
        { }

        public string IdNota { get; init; }

        public TipologiaVisibilitaNotaEnum TipologiaVisibilita { get; init; }

        public TextValue Testo { get; init; }
    }

    public class DataScadenzaAssignedEvent : Event
    {
        public DataScadenzaAssignedEvent()
        { }

        public DateTime? NewDataScadenza { get; init; }
    }
}
