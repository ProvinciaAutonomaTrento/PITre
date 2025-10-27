// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getElementiRubricaVeloceRequest = Pi3.App.Legacy.WebApi.Application.Requests.getElementiRubricaVeloce;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getElementiRubricaVeloce
{
    public class getElementiRubricaVeloceHandler : IRequestHandler<getElementiRubricaVeloceRequest, getElementiRubricaVeloceResult>
    {
        #region Public Members

        public getElementiRubricaVeloceHandler(ILogger<getElementiRubricaVeloceHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<getElementiRubricaVeloceResult> Handle(getElementiRubricaVeloceRequest request, CancellationToken cancellationToken)
        {
            List<string> output = new();
            try
            {
                var ers = (await this._mediator.Send(new Application.Requests.rubricaGetElementiRubrica(request.qco,request.infoUtente,new()))).output;

                string tempStringElemento = null;

                for (int i = 0; i < ers.Count(); i++)
                {
                    DocsPaVO.rubrica.ElementoRubrica tempElement = (DocsPaVO.rubrica.ElementoRubrica)ers[i];
                    string codRegTemp = tempElement.codiceRegistro;
                    if (tempElement.isRubricaComune == true)
                    {
                        codRegTemp = " [RC]";
                        if (!string.IsNullOrEmpty(tempElement.rubricaEsterna))
                            codRegTemp = " [" + tempElement.rubricaEsterna + "]";
                    }
                    else
                    {
                        if (codRegTemp == null || codRegTemp.Equals(""))
                        {
                            if (tempElement.interno == true || tempElement.tipo.Equals("L"))
                            {
                                codRegTemp = "";
                            }
                            else
                            {
                                codRegTemp = " [TUTTI]";
                            }
                        }
                        else
                        {
                            codRegTemp = " [" + tempElement.codiceRegistro + "]";
                        }
                    }

                    tempStringElemento = tempElement.descrizione + " (" + tempElement.codice + ")" + codRegTemp;
                    output.Add(tempStringElemento);
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getElementiRubricaVeloceHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}