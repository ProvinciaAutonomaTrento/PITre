using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.SignBook
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
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


        [DataMember]
        public string IdEvent { get; set; }
        [DataMember]
        public string CodeAction { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string EventType { get; set; }
        [DataMember]
        public string Group { get; set; }

        
    }
}