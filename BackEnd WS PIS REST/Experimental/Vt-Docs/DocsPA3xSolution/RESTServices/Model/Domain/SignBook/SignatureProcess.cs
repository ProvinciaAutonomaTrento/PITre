using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain.SignBook
{
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
                if (input.passi != null && input.passi.Count > 0)
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


        public string IdProcess { get; set; }
        public string Name { get; set; }
        public string AuthorRoleId { get; set; }
        public string AuthorUserId { get; set; }
        public List<SignatureStep> Steps { get; set; }
        public bool isInvalidated { get; set; }
        public bool IsProcessModel { get; set; }
    }
}