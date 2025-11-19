// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook
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
