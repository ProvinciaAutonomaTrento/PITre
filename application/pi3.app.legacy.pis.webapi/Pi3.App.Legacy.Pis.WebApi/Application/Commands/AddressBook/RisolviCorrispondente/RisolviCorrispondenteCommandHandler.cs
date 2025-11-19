// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.areaConservazione;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente
{
    public class RisolviCorrispondenteCommandHandler : IRequestHandler<RisolviCorrispondenteCommand, RisolviCorrispondenteCommandResponse>
    {
        public RisolviCorrispondenteCommandHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<RisolviCorrispondenteCommandResponse> Handle(RisolviCorrispondenteCommand request, CancellationToken cancellationToken)
        {
            RisolviCorrispondenteCommandResponse output = new();
            DocsPaVO.utente.Corrispondente corr = null;
            var corrResp = await this._mediator.Send(new AddressbookGetCorrispondenteBySystemIdCommand()
            {
                SystemId = request.SearchKey
            });
            if(corrResp != null)
            {
                corr = corrResp.Output;
            }
            
            if(corr == null || string.IsNullOrEmpty(corr.systemId))
            {
                var corrRcResp = await this._mediator.Send(new GetCorrispondenteByCodRubricaRubricaComuneCommand()
                {
                    Codice = request.SearchKey,
                    InfoUtente = request.InfoUtente
                });
                if(corrRcResp != null)
                {
                    corr = corrRcResp.Output;
                    if (corr != null && string.IsNullOrEmpty(corr.systemId))
                        corr = null;
                }
            }
            output.Corrispondente = corr;
            return output;
        }

        #region Private Members
        protected readonly IMediator _mediator;

        #endregion
    }
}
