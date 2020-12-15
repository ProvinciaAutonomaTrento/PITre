using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain.SignBook
{
    public class Event
    {
        public Event() { }

        public Event(DocsPaVO.LibroFirma.Evento evento)
        {
            if (evento != null)
            {
                this.IdEvent = evento.IdEvento;
                this.CodeAction = evento.CodiceAzione;
                this.Description = evento.Descrizione;
                this.EventType = evento.TipoEvento;
                this.Group = evento.Gruppo;
            }
        }


        public string IdEvent { get; set; }
        public string CodeAction { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public string Group { get; set; }
    }
}