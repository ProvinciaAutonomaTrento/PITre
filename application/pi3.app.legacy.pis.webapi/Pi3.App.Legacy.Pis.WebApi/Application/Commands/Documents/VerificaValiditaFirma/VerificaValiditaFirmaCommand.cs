// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.VerificaValiditaFirma
{
    public class VerificaValiditaFirmaCommand : IRequest<VerificaValiditaFirmaCommandResponse>
    {

        public DocsPaVO.documento.FileDocumento FileDoc { get; set; }
        public DocsPaVO.utente.InfoUtente InfoUtente { get; set; }
        public DateTime? DataDiVerifica { get; set; }

    }

    public class VerificaValiditaFirmaCommandResponse
    {
        public bool Output { get; set; }
    }
}
