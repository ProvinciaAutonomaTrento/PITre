// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileDocumentById;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.VerificaValiditaFirma
{
    public class VerificaValiditaFirmaCommandHandler : IRequestHandler<VerificaValiditaFirmaCommand, VerificaValiditaFirmaCommandResponse>
    {
        public VerificaValiditaFirmaCommandHandler(
            IMediator mediator,
            IPi3DbContext dbContext,
            IFirmaDigitale2Service firmaDigitale2Service
            )
        {
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._firmaDigitale2Service = firmaDigitale2Service;
        }

        public async Task<VerificaValiditaFirmaCommandResponse> Handle(VerificaValiditaFirmaCommand request, CancellationToken cancellationToken)
        {
            bool valid = false;

            var verificaFirmaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = request.FileDoc.fullName.ToUpper().EndsWith("P7M"),
                FileFirmato = request.FileDoc.content,
                DataVerifica = request.DataDiVerifica,
                TipoVerifica = TipiVerifica.Appiattita,
                ReturnFileOriginale = false,
                ReturnXmlCompleto = true
            });
            if (verificaFirmaResponse.Esito != null)
                valid = true;

            return new() 
            {
                Output = valid
            };
        }



        #region Private Members

        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        #endregion
    }
}
