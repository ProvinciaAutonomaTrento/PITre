using RESTServices.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain.SignBook
{
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
                if (input.RuoloProponente != null) this.ProposerRole = Utils.getRole(input.RuoloProponente);
                if (input.UtenteProponente != null) this.ProposerUser = Utils.getUser(input.UtenteProponente);
                this.DelegateUserDescription = input.DescUtenteDelegato;

                this.VersionNumber = input.numeroVersione;
                this.InterruptedNotification = input.Notifica_interrotto;
                this.EndingNotification = input.Notifica_concluso;
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

        public string IdProcessInstance { get; set; }
        public string IdProcess { get; set; }
        public string ProcessState { get; set; }
        public string AttachmentDoc { get; set; }
        public string DocNumber { get; set; }
        public string CreationDate { get; set; }
        public string ProtocolNumber { get; set; }
        public string ProtocolDate { get; set; }
        public string RepertorySignature { get; set; }
        public string Object { get; set; }
        public string VersionId { get; set; }
        public string ActiovationDate { get; set; }
        public string ClosureDate { get; set; }
        public string AttachmentNumber { get; set; }
        public Domain.Role ProposerRole { get; set; }
        public Domain.User ProposerUser { get; set; }
        public string DelegateUserDescription { get; set; }
        public int VersionNumber { get; set; }
        public bool InterruptedNotification { get; set; }
        public bool EndingNotification { get; set; }
        public string Description { get; set; }
        public List<Domain.SignBook.SignStepInstance> SignStepInstances { get; set; }
        public string StartingNote { get; set; }
        public string RefusalReason { get; set; }
        public char InterruptedBy { get; set; }
        public bool Truncated { get; set; }

    }
}