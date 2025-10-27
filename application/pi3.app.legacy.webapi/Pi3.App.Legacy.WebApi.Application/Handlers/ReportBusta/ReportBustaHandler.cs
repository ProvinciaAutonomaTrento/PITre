// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using DocsPaVO.documento;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportBustaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ReportBusta;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReportBusta
{
    public class ReportBustaHandler : IRequestHandler<ReportBustaRequest, ReportBustaResult>
    {
        #region Public Members

        public ReportBustaHandler(ILogger<ReportBustaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<ReportBustaResult> Handle(ReportBustaRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = new();
            try
            {
                output = await this.GetBusta(request.schedaDoc);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ReportBustaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private async Task<FileDocumento> GetBusta(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            DocsPaVO.documento.FileDocumento result = new DocsPaVO.documento.FileDocumento();
            var report = Resources.headerBusta;
            var bottomBusta = Resources.bottomBusta;

            var destinatari = ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.ToList();
            bool isFirstDest = true;
            foreach(var dest in destinatari)
            {
                var bodyBusta = Resources.bodyBusta;
                bodyBusta = bodyBusta.Replace("XNOME_REG", schedaDoc.registro.descrizione);
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.systemId = dest.systemId;
                var dettDest = await this.GetDettagliCorr(qco);
                bodyBusta = bodyBusta.Replace("XNOME_DEST", dest.descrizione);
                bodyBusta = bodyBusta.Replace("XINDIRIZZO_DEST", dettDest.Corrispondente[0].indirizzo);

                string citta = "";
                if (dettDest.Corrispondente[0].cap != null)
                {
                    citta = citta + dettDest.Corrispondente[0].cap + "   ";
                };
                if (dettDest.Corrispondente[0].citta != null)
                {
                    citta = citta + dettDest.Corrispondente[0].citta + "   ";
                };
                if (dettDest.Corrispondente[0].nazione != null)
                {
                    citta = citta + "(" + dettDest.Corrispondente[0].nazione + ")";
                };
                bodyBusta = bodyBusta.Replace("XCITTA_DEST", citta);

                if(!isFirstDest)
                    bodyBusta = bodyBusta.Replace("XNEW_PAGE", "{\\b0\\lang1040\\langfe1033\\langnp1040 \\page }");
                else
                    bodyBusta = bodyBusta.Replace("XNEW_PAGE", "");


                report = report + bodyBusta;
                isFirstDest = false;
            }

            report = report + bottomBusta;
            result.content = this.ToByteArray(report);
            result.length = result.content.Length;
            result.contentType = "application/rtf";
            result.name = "report.rtf";

            return result;

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

        private async Task<DettagliCorrispondente> GetDettagliCorr(DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente)
        {
            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();

            var dett = await (from a in this._dbContext.DettGlobaliEntities.AsNoTracking()
                              where a.ID_CORR_GLOBALI == objQueryCorrispondente.systemId.AsLong()
                              select a).FirstOrDefaultAsync();

            if(dett != null )
            {
                dettagliCorr.Corrispondente.AddCorrispondenteRow(dett.VAR_INDIRIZZO ?? string.Empty,
                                                                     dett.VAR_CITTA ?? string.Empty,
                                                                     dett.VAR_CAP ?? string.Empty,
                                                                     dett.VAR_PROVINCIA ?? string.Empty,
                                                                     dett.VAR_NAZIONE ?? string.Empty,
                                                                     dett.VAR_TELEFONO ?? string.Empty,
                                                                     dett.VAR_TELEFONO2 ?? string.Empty,
                                                                     dett.VAR_FAX ?? string.Empty,
                                                                     dett.VAR_COD_FISC ?? string.Empty,
                                                                     dett.VAR_NOTE ?? string.Empty,
                                                                     dett.VAR_LOCALITA ?? string.Empty,
                                                                     dett.VAR_LUOGO_NASCITA ?? string.Empty,
                                                                     dett.DTA_NASCITA != null ? dett.DTA_NASCITA : string.Empty,
                                                                     dett.VAR_TITOLO ?? string.Empty,
                                                                     dett.VAR_COD_PI ?? string.Empty);
            }
            else{
                dettagliCorr.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            return dettagliCorr;
        }





        #endregion
    }
}