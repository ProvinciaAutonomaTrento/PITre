// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsertMailCorrispondenteEsternoRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertMailCorrispondenteEsterno;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertMailCorrispondenteEsterno
{
    public class InsertMailCorrispondenteEsternoHandler : IRequestHandler<InsertMailCorrispondenteEsternoRequest, InsertMailCorrispondenteEsternoResult>
    {

        #region Private Members

        protected readonly ILogger<InsertMailCorrispondenteEsternoHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;

        private async Task<bool> InsertMailCorr(List<MailCorrispondente> listCaselleCorr, string idCorrispondente)
        {
            bool res = false;

            // Elimino tutte le caselle precedentemente associate al corrispondente esterno
            var isDeleteSucc = ( await this._mediator.Send(new Application.Requests.DeleteMailCorrispondenteEsterno(idCorrispondente)) ).output;

            if (!isDeleteSucc)
            {
                throw new Exception();
            }
            else
            {
                // Inserisco tutte le caselle
                MailCorrEsterniEntity mailCorrEsterniEntity = new();
                foreach (MailCorrispondente c in listCaselleCorr)
                {
                    mailCorrEsterniEntity = new()
                    {
                        ID_CORR = idCorrispondente.AsLong(),
                        VAR_EMAIL = c.Email.Trim(),
                        VAR_PRINCIPALE = c.Principale,
                        VAR_NOTE = string.IsNullOrEmpty(c.Note) ? c.Note : c.Note.Replace("'", "''"),
                    };

                    this._dbContext.MailCorrEsterniEntities.Add(mailCorrEsterniEntity);
                }

                long rowAffected = await((DbContext)this._dbContext).SaveChangesAsync();

                if(rowAffected > 0 || listCaselleCorr.Count==0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            }

            return res;
        }
        #endregion
        public InsertMailCorrispondenteEsternoHandler(
            ILogger<InsertMailCorrispondenteEsternoHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
        }
        public async Task<InsertMailCorrispondenteEsternoResult> Handle(InsertMailCorrispondenteEsternoRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.InsertMailCorr(request.listCaselle,request.idCorrispondente);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex , message: ex.Message);
            }
            return new(output);
        }
    }
}
