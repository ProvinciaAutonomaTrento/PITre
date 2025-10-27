// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using reportFascetteFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.reportFascetteFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.reportFascetteFascicolo
{
    public class reportFascetteFascicoloHandler : IRequestHandler<reportFascetteFascicoloRequest, reportFascetteFascicoloResult>
    {
        #region Public Members

        public reportFascetteFascicoloHandler(ILogger<reportFascetteFascicoloHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<reportFascetteFascicoloResult> Handle(reportFascetteFascicoloRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.FileDocumento retValue = null;
            DocsPaVO.fascicolazione.Fascicolo fascicolo = request.fascicolo;

            try
            {
                retValue = await this.StampaFascetta(fascicolo);
                if (retValue != null)
                    await this._webMethodLoggerService.LogOK("REGISTRIDISTAMPA", fascicolo.systemID, string.Format(Resources.LogStampaFascette, fascicolo.codice));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("REGISTRIDISTAMPA", fascicolo.systemID, string.Format(Resources.LogStampaFascette, fascicolo.codice));
            }


            return new reportFascetteFascicoloResult(retValue);
        }



        #endregion

        #region Private Members  
        private async Task<FileDocumento> StampaFascetta(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {

            byte[] fileContent = this.GetReportData(fascicolo.codice.Replace("\\", "\\\\"), fascicolo.descrizione.Replace("\\", "\\\\"));

            return new FileDocumento()
            {
                content = fileContent,
                length = fileContent.Length,
                contentType = "application/rtf",
                name = Resources.Filename,
                path = "",
                fullName = '\u005C'.ToString() + Resources.Filename
            };
        }

        private byte[] GetReportData(string codice, string descrizione)
        {
            var report = Files.Fascetta;
            report = report.Replace("XCODICE", codice);
            report = report.Replace("XDESCRIZIONE", descrizione);

            return this.ToByteArray(report);
        }

        private byte[] ToByteArray(string str)
        {
            char[] charArray = str.ToCharArray();
            System.Collections.ArrayList byteArr = new System.Collections.ArrayList();
            for (int i = 0; i < charArray.Length; i++)
            {
                if ((int)charArray[i] > 255)
                {
                    string utf = "\\u" + ((int)charArray[i]) + "G";
                    char[] utfChars = utf.ToCharArray();
                    for (int j = 0; j < utfChars.Length; j++)
                    {
                        byteArr.Add((byte)utfChars[j]);
                    }
                }
                else
                {
                    byteArr.Add((byte)charArray[i]);
                }
            }
            byte[] res = (byte[])byteArr.ToArray(typeof(byte));
            return res;
        }

        protected readonly ILogger<reportFascetteFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}