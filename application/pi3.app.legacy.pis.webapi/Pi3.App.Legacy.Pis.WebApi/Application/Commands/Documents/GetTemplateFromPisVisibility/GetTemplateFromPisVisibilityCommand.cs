// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility
{
    public record GetTemplateFromPisVisibilityCommand(Template templatePis, DocsPaVO.ProfilazioneDinamica.Templates template, 
        bool search, string idRuolo, string DoP, string codeApplication, DocsPaVO.utente.InfoUtente infoUtente = null,
        File docPrincipale = null, string codeRegister = null, string codeRF = null, bool editDocument = false, 
        string idSdi = null) : IRequest<GetTemplateFromPisVisibilityCommandResponse>;


    public record GetTemplateFromPisVisibilityCommandResponse(DocsPaVO.ProfilazioneDinamica.Templates output);

}
