// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione
{
    public record SpedisciDocumentoResult(SpedizioneDocumento output);

    public record SpedisciDocumento(InfoUtente infoUtente, SchedaDocumento documento, SpedizioneDocumento infoSpedizione) : IRequest<SpedisciDocumentoResult>;
}
