using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.SignBook
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class SignatureProcess
    {
        public SignatureProcess() { }
        public SignatureProcess(DocsPaVO.LibroFirma.ProcessoFirma input)
        {
            if (input != null)
            {
                this.IdProcess = input.idProcesso;
                this.Name = input.nome;
                this.AuthorRoleId = input.idRuoloAutore;
                this.AuthorUserId = input.idPeopleAutore;
                if (input.passi != null && input.passi.Count>0)
                {
                    this.Steps = new List<SignatureStep>();
                    SignatureStep stepD = null;
                    foreach (DocsPaVO.LibroFirma.PassoFirma passo in input.passi)
                    {
                        stepD = new SignatureStep(passo);
                        this.Steps.Add(stepD);
                    }
                }
                this.isInvalidated = input.isInvalidated;
                this.IsProcessModel = input.IsProcessModel;
            }
        }
    

        [DataMember]
        public string IdProcess { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string AuthorRoleId{ get; set; }
        [DataMember]
        public string AuthorUserId { get; set; }
        [DataMember]
        public List<SignatureStep> Steps { get; set; }
        [DataMember]
        public bool isInvalidated { get; set; }
        [DataMember]
        public bool IsProcessModel { get; set; }
    }
}