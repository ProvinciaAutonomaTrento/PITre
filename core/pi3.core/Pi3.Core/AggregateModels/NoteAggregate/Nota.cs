// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Metadata;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NoteAggregate
{

    public class TipoAccessoChanged : Event
    {
        //dichiarazione Evento
        public TipoAccessoChanged()
        { }
        public string Id { get; init; }

        public string? IdAccessoRF { get; init; }

        public TipoAccessoNotaEnum TipoAccesso { get; init; }
    }

    //dichiarazione evento, usiamo ElementCreatedEvent invece di Event in quanto la sua chiamata produce la creazione dell'element Nota sottostante, 
    //viene richiamata dall'Handler che a sua volta chiama l'evento "base.Handler"  ElementCreatedEvent
    public class NotaCreata : ElementCreatedEvent
    {
        public NotaCreata()
        {         
        }

        public string? IdIdAccessoRF { get; init; }

        public AutoreNota Autore { get; init; }

        public string IdOggettoAssociato { get; init; }

        public TipiOggettoEnum TipoOggettoAssociato { get; init; }

        public TipoAccessoNotaEnum TipoAccesso { get; init; }

        public ElementCreatedEvent ElementCreated { get; init; }

        

    }

    public class Nota : Element
    {
        public Nota(string id, string idTenant, DateTime creationDate,TextValue nome ,TextValue? description, AutoreNota autore, string idOggetto, TipiOggettoEnum tipoOggetto, TipoAccessoNotaEnum tipoAccesso, string? idAccessoRF) 
        {
            //chiediamo di eseguire la chiamata all'evento NotaCreata, oltre agli elementi richiesti sopra, 
            //ci sono alcune elementi di base (idTenant, creationDate, name(valore testuale dell'oggetto), Descrizione(se serve)) che devono essere aggiunti, nel caso specifico name contiene il testo della nota
            this.ApplyChange(new NotaCreata()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = "Nota",
                CreationDate = creationDate,
                Name = nome,
                Description = description,
                Autore = autore,
                IdOggettoAssociato = idOggetto,
                TipoOggettoAssociato= tipoOggetto,
                TipoAccesso = tipoAccesso,
                IdIdAccessoRF = idAccessoRF
            }); 
        }
        
        //la medesima chiamata di sopra, ma senza id, serve ne caso in cui la chiamata crei la nota (quindi non si ha ancora un ID), si usa un passaggio di parametri particolare 
        //passando come parametri i valori del costruttore. 
        public Nota(string idTenant, DateTime creationDate, TextValue name,TextValue? description, AutoreNota autore, string idOggetto, TipiOggettoEnum tipoOggetto, TipoAccessoNotaEnum tipoAccesso, string? idAccessoRF) 
            : this(null,  idTenant,  creationDate,  name, description,  autore, idOggetto, tipoOggetto,  tipoAccesso, idAccessoRF)
        {           
        }

        //dichiariamo i componenti dell'oggetto che verranno utilizzati nelle chiamate degli oggetti dell'elemento
        public TipoAccessoNotaEnum TipoAccesso { get; protected set; }
        public string? IdAccessoRF { get; protected set; }
        public AutoreNota Autore { get; protected set; }

        public OggettoAssociato OggettoAssociato { get; protected set; }    

        //creazione dei metodi
        public void SetAccessoRF(string idAccessoRF)
        {
            if(TipoAccesso > TipoAccessoNotaEnum.RF)          
                throw new NotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipoAccessoChanged() { Id=this.Id, IdAccessoRF = idAccessoRF, TipoAccesso = TipoAccessoNotaEnum.RF });
        }

        public void SetAccessoRuolo()
        {
            if (TipoAccesso > TipoAccessoNotaEnum.Ruolo)
                throw new NotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipoAccessoChanged() { Id = this.Id, IdAccessoRF = null, TipoAccesso = TipoAccessoNotaEnum.Ruolo });
        }

        public void SetAccessoPersonale()
        {
            if (TipoAccesso > TipoAccessoNotaEnum.Personale)
                throw new NotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipoAccessoChanged() { Id = this.Id, IdAccessoRF = null, TipoAccesso = TipoAccessoNotaEnum.Personale });
        }

        public void SetAccessoPubblico()
        {
            this.ApplyChange(new TipoAccessoChanged() { Id = this.Id, IdAccessoRF = null, TipoAccesso = TipoAccessoNotaEnum.Pubblica });
        }

        //qui creiamo l'oggetto che effettua "fisicamente" la chiamata
        //@event. associa gli oggetti dichiarati nell'ellement con quelli dell'evento interessato
        protected void Handle(TipoAccessoChanged @event)
        {
            this.TipoAccesso = @event.TipoAccesso;
            this.IdAccessoRF = @event.IdAccessoRF;
        }    

        //l'handler che si occupa effettivamente della creazione
        protected void Handle(NotaCreata @event)
        {
            base.Handle((ElementCreatedEvent) @event);
            this.Autore = @event.Autore;
            this.OggettoAssociato = new OggettoAssociato(@event.IdOggettoAssociato, @event.TipoOggettoAssociato);
            this.TipoAccesso = @event.TipoAccesso;
            this.IdAccessoRF = @event.IdIdAccessoRF;    
            
        }
    }  

    //dichiariamo un oggetto dell'aggregate richiesto nell'element, nel caso specifico Autore (valueObject)
    public class AutoreNota : ValueObject
    {
        public AutoreNota()
        { }

        [Required(AllowEmptyStrings = false)]
        public string IdUtente { get; init; }
        public string IdRuolo { get; init; }
        public string? IdUtenteDelegato { get; init; }
    }

    //creiamo gli eventuali oggetti Entity, nel nostro caso sono Documento e Fascicoli
    public class OggettoAssociato : Entity<string>
    {
        #region Public Members

        internal OggettoAssociato(string idOggetto, TipiOggettoEnum tipoOggetto)
        {
            this.Id = idOggetto;
            this.TipoOggetto = tipoOggetto;
        }

        //dichiariamo gli elementi "singoli" come eventuali string, int o enum ecc.
        public  TipiOggettoEnum TipoOggetto { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }

   

}
