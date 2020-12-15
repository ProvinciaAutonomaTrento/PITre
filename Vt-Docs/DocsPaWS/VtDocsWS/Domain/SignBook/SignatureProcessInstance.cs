using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.SignBook
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class SignatureProcessInstance
    {
        public SignatureProcessInstance() { }
        public SignatureProcessInstance(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma input)
        {
            if (input != null)
            {
                this.IdProcessInstance = input.idIstanzaProcesso;
                this.IdProcess = input.idProcesso;
                if (input.statoProcesso != null)
                {
                    switch (input.statoProcesso)
                    {
                        case DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED: this.ProcessState = "CLOSED"; break;
                        case DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED_WITH_CUT: this.ProcessState = "CLOSED_WITH_CUT"; break;
                        case DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC: this.ProcessState = "IN_EXEC"; break;
                        case DocsPaVO.LibroFirma.TipoStatoProcesso.STOPPED: this.ProcessState = "STOPPED"; break;
                    }
                }
                this.AttachmentDoc = input.docAll;
                this.DocNumber = input.docNumber;
                this.CreationDate = input.DataCreazione;
                this.ProtocolNumber = input.NumeroProtocollo;
                this.ProtocolDate = input.DataProtocollazione;
                this.RepertorySignature = input.SegnaturaRepertorio;
                this.Object = input.oggetto;
                this.VersionId = input.versionId;
                this.ActiovationDate = input.dataAttivazione;
                this.ClosureDate = input.dataChiusura;
                this.AttachmentNumber = input.numeroAllegato;
                if (input.RuoloProponente != null) this.ProposerRole = VtDocsWS.WebServices.Utils.getRole(input.RuoloProponente);
                if (input.UtenteProponente != null) this.ProposerUser = VtDocsWS.WebServices.Utils.getUser(input.UtenteProponente);
                this.DelegateUserDescription = input.DescUtenteDelegato;

                this.VersionNumber = input.numeroVersione;
                if (input.Notifiche != null)
                {
                    this.InterruptedNotification = input.Notifiche.Notifica_interrotto;
                    this.EndingNotification = input.Notifiche.Notifica_concluso;
                }
                this.Description = input.Descrizione;
                if (input.istanzePassoDiFirma != null && input.istanzePassoDiFirma.Count > 0)
                {
                    this.SignStepInstances = new List<SignStepInstance>();
                    SignStepInstance instD = null;
                    foreach (DocsPaVO.LibroFirma.IstanzaPassoDiFirma istanza in input.istanzePassoDiFirma)
                    {
                        instD = new SignStepInstance(istanza);
                        this.SignStepInstances.Add(instD);
                    }
                }
                this.StartingNote = input.NoteDiAvvio;
                this.RefusalReason = input.MotivoRespingimento;
                this.InterruptedBy = input.ChaInterroDa;
                this.Truncated = input.IsTroncato;
            }
        }

        [DataMember]
        public string IdProcessInstance { get; set; }
        [DataMember]
        public string IdProcess { get; set; }
        [DataMember]
        public string ProcessState { get; set; }
        [DataMember]
        public string AttachmentDoc { get; set; }
        [DataMember]
        public string DocNumber { get; set; }
        [DataMember]
        public string CreationDate { get; set; }
        [DataMember]
        public string ProtocolNumber { get; set; }
        [DataMember]
        public string ProtocolDate { get; set; }
        [DataMember]
        public string RepertorySignature { get; set; }
        [DataMember]
        public string Object { get; set; }
        [DataMember]
        public string VersionId { get; set; }
        [DataMember]
        public string ActiovationDate { get; set; }
        [DataMember]
        public string ClosureDate { get; set; }
        [DataMember]
        public string AttachmentNumber { get; set; }
        [DataMember]
        public VtDocsWS.Domain.Role ProposerRole { get; set; }
        [DataMember]
        public VtDocsWS.Domain.User ProposerUser { get; set; }
        [DataMember]
        public string DelegateUserDescription { get; set; }
        [DataMember]
        public int VersionNumber { get; set; }
        [DataMember]
        public bool InterruptedNotification { get; set; }
        [DataMember]
        public bool EndingNotification { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<Domain.SignBook.SignStepInstance> SignStepInstances { get; set; }
        [DataMember]
        public string StartingNote { get; set; }
        [DataMember]
        public string RefusalReason { get; set; }
        [DataMember]
        public char InterruptedBy { get; set; }
        [DataMember]
        public bool Truncated { get; set; }

    }
}