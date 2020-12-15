using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.SignBook
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
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
                if (input.ruoloCoinvolto != null) this.InvolvedRole = VtDocsWS.WebServices.Utils.getRole(input.ruoloCoinvolto);
                if (input.utenteCoinvolto != null) this.InvolvedUser = VtDocsWS.WebServices.Utils.getUser(input.utenteCoinvolto);
                this.Note = input.note;
                this.EventsToNotifyIds = input.idEventiDaNotificare;
                this.ExpirationDate = input.dataScadenza;
                this.Invalidated = input.Invalidated;
                this.IsModel = input.IsModello;
                this.ToUpdate = input.DaAggiornare;
            }
        }

        [DataMember]
        public string IdStep { get; set; }
        [DataMember]
        public string IdProcess { get; set; }
        [DataMember]
        public int SequenceNumber { get; set; }
        [DataMember]
        public Event Event { get; set; }
        [DataMember]
        public string InvolvedRoleTypeIdAmm { get; set; }
        [DataMember]
        public string InvolvedRoleTypeCode { get; set; }
        [DataMember]
        public string InvolvedRoleType { get; set; }
        [DataMember]
        public VtDocsWS.Domain.Role InvolvedRole { get; set; }
        [DataMember]
        public VtDocsWS.Domain.User InvolvedUser { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public List<string> EventsToNotifyIds { get; set; }
        [DataMember]
        public string ExpirationDate { get; set; }
        [DataMember]
        public char Invalidated { get; set; }
        [DataMember]
        public bool IsModel { get; set; }
        [DataMember]
        public bool ToUpdate { get; set; }
    }
}