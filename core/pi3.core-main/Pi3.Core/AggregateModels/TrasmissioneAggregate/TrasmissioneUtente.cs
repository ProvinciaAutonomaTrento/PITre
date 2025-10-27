// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{

    public class Accetta : ValueObject
    {
        public Accetta()
        { }

        public DateTime? Data { get; init; } = null;

        public TextValue? Note { get; init; } = null;

        public string? IdDelegato { get; init; } = null;

        public string? UserIdDelegato { get; init; } = null;

        public string? CognomeDelegato { get; init; } = null;

        public string? NomeDelegato { get; init; } = null;
    }

    public class Rifiuta : ValueObject
    {
        public Rifiuta()
        { }

        public DateTime? Data { get; init; } = null;

        [Required]
        public TextValue Note { get; init; }

        public string? IdDelegato { get; init; } = null;

        public string? UserIdDelegato { get; init; } = null;

        public string? CognomeDelegato { get; init; } = null;

        public string? NomeDelegato { get; init; } = null;
    }

    public class Visto : ValueObject
    {
        public Visto()
        { }

        public DateTime? Data { get; init; } = null;

        public string? IdDelegato { get; init; } = null;

        public string? UserIdDelegato { get; init; } = null;

        public string? CognomeDelegato { get; init; } = null;

        public string? NomeDelegato { get; init; } = null;
    }

    public class TrasmissioneUtente : Entity<string>
    {
        #region Public Members

        internal TrasmissioneUtente(
            TrasmissioneSingola parent,
            string id, UtenteDestinatario utenteDestinatario)
        {
            this.Parent = parent;
            this.Id = id;
            this.UtenteDestinatario = utenteDestinatario;
        }

        public TrasmissioneSingola Parent { get; protected set; }

        public UtenteDestinatario UtenteDestinatario { get; protected set; }
        
        public DateTime? DataVista { get; protected set; } = null;

        public DateTime? DataAccettazione { get; protected set; } = null;

        public TextValue? NoteAccettazione { get; protected set; } = null;

        public DateTime? DataRifiuto { get; protected set; } = null;

        public TextValue? NoteRifiuto { get; protected set; } = null;

        public DateTime? DataRimozioneCentroNotifiche { get; protected set; } = null;

        internal void SetDataRimozioneCentroNotifiche(DateTime? dataRimozione)
        {
            this.DataRimozioneCentroNotifiche = dataRimozione;
        }

        public UtenteDestinatario? UtenteDelegato { get; protected set; } = null;

        //internal void Do(Visto visto)
        //{
        //    if (this.DataVista.HasValue)
        //        throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaVista, ErrorDescriptions.ResourceManager, this.Id);

        //    this.DataVista = visto.Data;
            
        //    if (!string.IsNullOrWhiteSpace(visto.IdDelegato))
        //        this.UtenteDelegato = new UtenteDestinatario(visto.IdDelegato, visto.UserIdDelegato, visto.CognomeDelegato, visto.NomeDelegato);
        //}

        internal void Do(Accetta accetta)
        {
            if (!this.Parent.RagioneTrasmissione.ConWorkflow ?? false)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.RagioneTrasmissioneSenzaWorkflow, ErrorDescriptions.ResourceManager, this.Id);
            
            if (this.Parent.Tipo == TipiTrasmissioneSingolaEnum.Uno && 
                this.Parent.TrasmissioniUtente.Any(tu => tu.DataAccettazione.HasValue))
                    throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaAccettata, ErrorDescriptions.ResourceManager, this.Id);
            
            else if (this.DataAccettazione.HasValue)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaAccettata, ErrorDescriptions.ResourceManager, this.Id);

            this.DataAccettazione = accetta.Data;
            this.NoteAccettazione = accetta.Note;
            
            if (!string.IsNullOrWhiteSpace(accetta.IdDelegato))
                this.UtenteDelegato = new UtenteDestinatario(accetta.IdDelegato, accetta.UserIdDelegato, accetta.CognomeDelegato, accetta.NomeDelegato);
        }

        internal void Do(Rifiuta rifiuta)
        {
            if (!this.Parent.RagioneTrasmissione.ConWorkflow ?? false)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.RagioneTrasmissioneSenzaWorkflow, ErrorDescriptions.ResourceManager, this.Id);

            if (this.Parent.Tipo == TipiTrasmissioneSingolaEnum.Uno &&
                            this.Parent.TrasmissioniUtente.Any(tu => tu.DataRifiuto.HasValue))
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaRifiutata, ErrorDescriptions.ResourceManager, this.Id);

            else if (this.DataRifiuto.HasValue)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaRifiutata, ErrorDescriptions.ResourceManager, this.Id);

            this.DataRifiuto = rifiuta.Data;
            this.NoteRifiuto = rifiuta.Note;

            if (!string.IsNullOrWhiteSpace(rifiuta.IdDelegato))
                this.UtenteDelegato = new UtenteDestinatario(rifiuta.IdDelegato, rifiuta.UserIdDelegato, rifiuta.CognomeDelegato, rifiuta.NomeDelegato);
        }

        #endregion

        #region Private Members

        #endregion
    }
}
