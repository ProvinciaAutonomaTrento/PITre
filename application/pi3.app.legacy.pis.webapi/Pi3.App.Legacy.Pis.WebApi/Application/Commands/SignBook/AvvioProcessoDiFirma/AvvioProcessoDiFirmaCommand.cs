// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcess;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma
{
    public class AvvioProcessoDiFirmaCommand : IRequest<AvvioProcessoDiFirmaCommandResponse>
    {
        public ProcessoFirma processoDiFirma { get; set; }
        public DocsPaVO.documento.FileRequest file { get; set; }
        public string modalita { get; set; }
        public string note { get; set; }
        public OpzioniNotifica opzioniNotifiche { get; set; }
        public bool daCambioStato { get; set; }
    }

    public class AvvioProcessoDiFirmaCommandResponse : MessageResponse
    {
        public bool output { get; set; }
        public ResultProcessoFirma resultAvvioProcesso { get; set; }
    }
}
