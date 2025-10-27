// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate
{

    public class ModelloTrasmissioneCreatoEvent : ElementCreatedEvent
    {
        public ModelloTrasmissioneCreatoEvent()
        { }

        public TextValue? NoteGenerali { get; init; }

        public string IdRegistro { get; init; }

        public string? CodiceRegistro { get; init; }

        public TextValue? DescrizioneRegistro { get; init; }

        public TipiOggettiTrasmessiEnum TipoOggettoTrasmesso { get; init; }
    }

    public class NoteGeneraliChangedEvent : Event
    {
        public NoteGeneraliChangedEvent()
        { }

        public string Id { get; init; }

        public TextValue? NewNoteGenerali { get; init; }
    }

    public class RegistroChangedEvent : Event
    {
        public RegistroChangedEvent()
        { }

        public string Id { get; init; }

        public string IdRegistro { get; init; }

        public string? CodiceRegistro { get; init; }

        public TextValue? DescrizioneRegistro { get; init; }
    }


    public class TipoOggettoTrasmessoChangedEvent : Event
    {
        public TipoOggettoTrasmessoChangedEvent()
        { }

        public string Id { get; init; }

        public TipiOggettiTrasmessiEnum NewTipoOggettoTrasmesso { get; init; }
    }

    public class GruppoDestinatarioAdded : Event
    {
        public GruppoDestinatarioAdded()
        { }

        public string Id { get; init; }

        public string IdGruppo { get; init; }

        public string? CodiceGruppo { get; init; }

        public TextValue? DescrizioneGruppo { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public CessioneDirittiRagioneTrasmissione? CessioneDirittiRagioneTrasmissione { get; init; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; init; }

        public List<DatiUtenteNotificato> UtentiNotificati { get; init; }

        public TextValue? NoteTrasmissioneSingola { get; init; }

        public int? GiorniScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }
    
    public class GruppoDestinatarioRemoved : Event
    {
        public GruppoDestinatarioRemoved()
        { }

        public string Id { get; init; }

        public string IdGruppo { get; init; }
    }

    public class UtenteDestinatarioAdded : Event
    {
        public UtenteDestinatarioAdded()
        { }

        public string Id { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public CessioneDirittiRagioneTrasmissione? CessioneDirittiRagioneTrasmissione { get; init; }

        public TextValue? NoteTrasmissioneSingola { get; init; }

        public int? GiorniScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }

    public class UtenteDestinatarioRemoved : Event
    {
        public UtenteDestinatarioRemoved()
        { }

        public string Id { get; init; }

        public string IdUtente { get; init; }
    }

    public class UfficioDestinatarioAdded : Event
    {
        public UfficioDestinatarioAdded()
        { }

        public string Id { get; init; }

        public string IdUfficio { get; init; }

        public string? CodiceUfficio { get; init; }

        public TextValue? DescrizioneUfficio { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; init; }

        public TextValue? NoteTrasmissioneSingola { get; init; }

        public int? GiorniScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }

    public class UfficioDestinatarioRemoved : Event
    {
        public UfficioDestinatarioRemoved()
        { }

        public string Id { get; init; }

        public string IdUfficio { get; init; }
    }

    public class OpzioniGruppoDestinatarioChangedEvent : Event
    {
        public OpzioniGruppoDestinatarioChangedEvent()
        { }

        public string Id { get; init; }

        public string IdGruppoDestinatario { get; init; }

        public TipiTrasmissioneSingolaEnum? TipoTrasmissioneSingola { get; init; } = null;

        public TextValue? NoteTrasmissioneSingola { get; init; } = null;

        public int? GiorniScadenza { get; init; } = null;

        public bool? NascondiVersioniPrecedenti { get; init; } = null;
    }

    public class OpzioniUtenteDestinatarioChangedEvent : Event
    {
        public OpzioniUtenteDestinatarioChangedEvent()
        { }

        public string Id { get; init; }

        public string IdUtenteDestinatario { get; init; }

        public TextValue? NoteTrasmissioneSingola { get; init; } = null;

        public int? GiorniScadenza { get; init; } = null;

        public bool? NascondiVersioniPrecedenti { get; init; } = null;
    }

    public class OpzioniUfficioDestinatarioChangedEvent : Event
    {
        public OpzioniUfficioDestinatarioChangedEvent()
        { }

        public string Id { get; init; }

        public string IdUfficioDestinatario { get; init; }

        public TipiTrasmissioneSingolaEnum? TipoTrasmissioneSingola { get; init; } = null;

        public TextValue? NoteTrasmissioneSingola { get; init; } = null;

        public int? GiorniScadenza { get; init; } = null;

        public bool? NascondiVersioniPrecedenti { get; init; } = null;
    }

    public class UtentiNotificatiChangedEvent : Event
    {
        public UtentiNotificatiChangedEvent()
        { }

        public string Id { get; init; }

        public string IdGruppoDestinatario { get; init; }

        public List<DatiUtenteNotificato> NewUtentiNotificati { get; init; }
    }

    public class AutorePersonaAssignedEvent : Event
    {
        public AutorePersonaAssignedEvent()
        { }

        public string Id { get; init; }

        public string? IdUser { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }
    }

    public class AutoreGruppoAssignedEvent : Event
    {
        public AutoreGruppoAssignedEvent()
        { }

        public string Id { get; init; }

        public string? IdGruppo { get; init; }

        public string? Codice { get; init; }

        public TextValue? Descrizione { get; init; }
    }


    public class CessioneDirittiEffettuataEvent : Event
    {
        public CessioneDirittiEffettuataEvent()
        { }

        public string Id { get; init; }

        public CessioneDiritti CessioneDiritti { get; init; }
    }
}
