// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.filtri;
using DocsPaVO.Security;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Security.InfoAtipicita;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record DocumentGetVisibilityWithFIlterResult(DocsPaVO.documento.DirittoOggetto[] output);

    public record DocumentGetVisibilityWithFIlter(InfoUtente infoUtente, string idProfile, bool cercaRimossi, FilterVisibility[] filters) : IRequest<DocumentGetVisibilityWithFIlterResult>;
    public record RipristinaACLWithTypeResult(bool output);

    public record RipristinaACLWithType(DocsPaVO.documento.DirittoOggetto docDiritto, string personOrGroup, InfoUtente infoUtente, string typeObject) : IRequest<RipristinaACLWithTypeResult>;
    public record GetInfoAtipicitaResult(InfoAtipicita output);

    public record GetInfoAtipicita(InfoUtente infoUtente, TipoOggettoAtipico tipoOggetto, string idDocOrFasc) : IRequest<GetInfoAtipicitaResult>;    
}
