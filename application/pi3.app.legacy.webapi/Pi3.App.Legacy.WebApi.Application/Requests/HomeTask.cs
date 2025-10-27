// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    //public record ChiudiLavorazioneTaskResult(bool output);

    //public record ChiudiLavorazioneTask(DocsPaVO.Task.Task task, string note, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ChiudiLavorazioneTaskResult>;
    //public record AnnullaTaskResult(bool output);

    //public record AnnullaTask(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AnnullaTaskResult>;
    //public record ChiudiTaskResult(bool output);

    //public record ChiudiTask(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ChiudiTaskResult>;
    //public record DocumentoGetDettaglioDocumentoResult(DocsPaVO.documento.SchedaDocumento output);

    //public record DocumentoGetDettaglioDocumento(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber) : IRequest<DocumentoGetDettaglioDocumentoResult>;
    //public record VerificaACLResult(int output);

    //public record VerificaACL(string tipoObj, string idObj, DocsPaVO.utente.InfoUtente infoUtente, out string errorMessage) : IRequest<VerificaACLResult>;
    //public record FascicolazioneGetFascicoloByIdResult(DocsPaVO.fascicolazione.Fascicolo output);

    //public record FascicolazioneGetFascicoloById(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<FascicolazioneGetFascicoloByIdResult>;
    //public record getTemplateFascDettagliResult(DocsPaVO.ProfilazioneDinamica.Templates output);

    //public record getTemplateFascDettagli(string idProject) : IRequest<getTemplateFascDettagliResult>;
    //public record RiapriLavorazioneResult(bool output);

    //public record RiapriLavorazione(DocsPaVO.Task.Task task, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo) : IRequest<RiapriLavorazioneResult>;
    //public record NewSchedaDocumentoResult(DocsPaVO.documento.SchedaDocumento output);

    //public record NewSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<NewSchedaDocumentoResult>;
    //public record getTemplateDettagliResult(DocsPaVO.ProfilazioneDinamica.Templates output);

    //public record getTemplateDettagli(string docNumber) : IRequest<getTemplateDettagliResult>;
    public record GetListaTaskRicevutiResult(DocsPaVO.Task.Task[] output);

    public record GetListaTaskRicevuti(bool incluteCompletedTasks, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetListaTaskRicevutiResult>;
    public record GetListaTaskAssegnatiResult(DocsPaVO.Task.Task[] output);

    public record GetListaTaskAssegnati(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetListaTaskAssegnatiResult>;

}
