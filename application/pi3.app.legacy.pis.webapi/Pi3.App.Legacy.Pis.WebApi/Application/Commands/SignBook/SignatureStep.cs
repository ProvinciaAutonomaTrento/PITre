// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook
{
    public class SignatureStep
    {
        public SignatureStep() { }
        public SignatureStep(DocsPaVO.LibroFirma.PassoFirma input)
        {
            if (input != null)
            {
                this.IdStep = input.idPasso;
                this.IdProcess = input.idProcesso;
                this.SequenceNumber = input.numeroSequenza;
                if (input.Evento != null) this.Event = new Event(input.Evento);
                if (input.TpoRuoloCoinvolto != null)
                {
                    this.InvolvedRoleTypeIdAmm = input.TpoRuoloCoinvolto.id_Amm;
                    this.InvolvedRoleTypeCode = input.TpoRuoloCoinvolto.codice;
                    this.InvolvedRoleType = input.TpoRuoloCoinvolto.descrizione;
                }
                if (input.ruoloCoinvolto != null) this.InvolvedRole = RestUtils.getRole(input.ruoloCoinvolto);
                if (input.utenteCoinvolto != null) this.InvolvedUser = RestUtils.getUser(input.utenteCoinvolto);
                this.Note = input.note;
                this.EventsToNotifyIds = input.idEventiDaNotificare;
                this.ExpirationDate = input.dataScadenza;
                this.Invalidated = input.Invalidated;
                this.IsModel = input.IsModello;
                this.ToUpdate = input.DaAggiornare;
            }
        }

        public string IdStep { get; set; }
        public string IdProcess { get; set; }
        public int SequenceNumber { get; set; }
        public Event Event { get; set; }
        public string InvolvedRoleTypeIdAmm { get; set; }
        public string InvolvedRoleTypeCode { get; set; }
        public string InvolvedRoleType { get; set; }
        public Role InvolvedRole { get; set; }
        public User InvolvedUser { get; set; }
        public string Note { get; set; }
        public List<string> EventsToNotifyIds { get; set; }
        public string ExpirationDate { get; set; }
        public char Invalidated { get; set; }
        public bool IsModel { get; set; }
        public bool ToUpdate { get; set; }
    }
}
