// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{
    public class TrasmissioneCreataEvent : ElementCreatedEvent
    {
        public TrasmissioneCreataEvent()
        { }

        public Autore? Autore { get; init; }

        public string IdOggettoTrasmesso { get; init; }

        public TipiOggettiTrasmessiEnum TipoOggettoTrasmesso { get; init; }

        public TextValue? NoteGenerali { get; init; }
    }


    public class NoteGeneraliChangedEvent : Event
    {
        public NoteGeneraliChangedEvent()
        { }

        public string Id { get; init; }

        public TextValue NewNoteGenerali { get; init; }
    }

    public class CediDirittiChangedEvent : Event
    {
        public CediDirittiChangedEvent()
        { }

        public string Id { get; init; }

        public bool NewCediDiritti { get; init; }
    }

    public class TrasmissioneInviataEvent : Event
    {
        public TrasmissioneInviataEvent()
        { }

        public string Id { get; init; }

        public DateTime DataInvio { get; init; }
    }

    public class TrasmissioneSingolaGruppoPreparedEvent : Event
    {
        public TrasmissioneSingolaGruppoPreparedEvent()
        {
        }

        public string Id { get; init; }

        public DatiTrasmissioneSingolaGruppo DatiTrasmissioneSingola { get; init; }
    }

    public class TrasmissioneSingolaUtentePreparedEvent : Event
    {
        public TrasmissioneSingolaUtentePreparedEvent()
        {
        }

        public string Id { get; init; }

        public DatiTrasmissioneSingolaUtente DatiTrasmissioneSingola { get; init; }
    }


    public class TrasmissioneSingolaGruppoAddedEvent : Event
    {
        public TrasmissioneSingolaGruppoAddedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public bool? RagioneConWorkflow { get; init; }

        public CessioneDirittiRagione? CessioneDirittiRagione { get; set; }

        public TipiTrasmissioneSingolaEnum Tipo { get; init; }

        public string IdGruppoDestinatario { get; init; }

        public string? CodiceGruppoDestinatario { get; init; }

        public TextValue? DescrizioneGruppoDestinatario { get; init; }

        public TextValue? Note { get; init; }

        public DateTime? DataScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }

    public class TrasmissioneSingolaUtenteAddedEvent : Event
    {
        public TrasmissioneSingolaUtenteAddedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public bool? RagioneConWorkflow { get; init; }
        
        public CessioneDirittiRagione? CessioneDirittiRagione { get; set; }

        public string IdUtenteDestinatario { get; init; }

        public string? UserIdDestinatario { get; init; }

        public string? CognomeDestinatario { get; init; }

        public string? NomeDestinatario { get; init; }

        public TextValue? Note { get; init; }

        public DateTime? DataScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }


    public class TrasmissioneSingolaRemovedEvent : Event
    {
        public TrasmissioneSingolaRemovedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }
    }

    public class TrasmissioneUtenteRemovedEvent : Event
    {
        public TrasmissioneUtenteRemovedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneUtente { get; init; }
    }

    public class TrasmissioneUtenteAddedEvent : Event
    {
        public TrasmissioneUtenteAddedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public string IdTrasmissioneUtente { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }

        public DateTime? DataRimozioneCentroNotifiche { get; init; }
    }


    public class TrasmissioneUtenteAccettataEvent : Event
    {
        public TrasmissioneUtenteAccettataEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneUtente { get; init; }

        public Accetta Accetta { get; init; }
    }

    public class TrasmissioneUtenteRifiutataEvent : Event
    {
        public TrasmissioneUtenteRifiutataEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneUtente { get; init; }

        public Rifiuta Rifiuta { get; init; }
    }

    public class TrasmissioneUtenteVistaEvent : Event
    {
        public TrasmissioneUtenteVistaEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneUtente { get; init; }

        public Visto Visto { get; init; }
    }


    public class TipoTrasmissioneSingolaChangedEvent : Event
    {
        public TipoTrasmissioneSingolaChangedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public TipiTrasmissioneSingolaEnum NewTipo { get; init; }
    }

    public class NoteTrasmissioneSingolaChangedEvent : Event
    {
        public NoteTrasmissioneSingolaChangedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public TextValue? NewNote { get; init; }
    }

    public class DataScadenzaTrasmissioneSingolaChangedEvent : Event
    {
        public DataScadenzaTrasmissioneSingolaChangedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public DateTime? NewDataScadenza { get; init; }
    }

    public class TrasmissioneSingolaNascondiVersioniPrecedentiChangedEvent : Event
    {
        public TrasmissioneSingolaNascondiVersioniPrecedentiChangedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public bool? NewNascondiVersioniPrecedenti { get; init; }
    }
}
