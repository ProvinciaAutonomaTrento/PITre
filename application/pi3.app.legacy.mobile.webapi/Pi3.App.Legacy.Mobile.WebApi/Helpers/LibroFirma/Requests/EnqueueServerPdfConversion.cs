// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Requests
{
    public record EnqueueServerPdfConversionResult();

    public record EnqueueServerPdfConversion(InfoUtente infoUtente, ObjServerPdfConversion objServerPdfConversion) : IRequest<EnqueueServerPdfConversionResult>;
}
