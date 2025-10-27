// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFileNoException;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;
using DocumentSaveTimestampRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentSaveTimestamp;
using Chilkat;
using DocsPaVO.areaConservazione;
using System.Globalization;
using System.Xml;
using Pi3.Core.Extensions;
using DocumentFormat.OpenXml.Office2013.Word;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocumentFormat.OpenXml.Bibliography;
using DeSign;
using System.Security.Cryptography.X509Certificates;
using Pi3.Core.Services.File.FirmaDigitale2;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentSaveTimestamp
{

    // Richiede libreria MediatR
    public class DocumentSaveTimestampHandler : IRequestHandler<DocumentSaveTimestampRequest, DocumentSaveTimestampResult>
    {
        #region Public Members

        public DocumentSaveTimestampHandler(ILogger<DocumentSaveTimestampHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IFirmaDigitale2Service firmaDigitale2Service)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._firmaDigitale2Service = firmaDigitale2Service;
        }

        public async Task<DocumentSaveTimestampResult> Handle(DocumentSaveTimestampRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);

            try
            {
                var verificaMarcaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
                {
                    VerificaCompleta = false,
                    FileFirmato = request.fileDocumento.content,
                    DataVerifica = DateTime.Now,
                    TipoVerifica = TipiVerifica.Esterna,
                    ReturnFileOriginale = false,
                    ReturnXmlCompleto = true
                });

                if (verificaMarcaResponse != null)
                {
                    DeSign.deSign datiGeneraliVerifica = null;
                    if (verificaMarcaResponse.Esito != null)
                    {
                        datiGeneraliVerifica = DeSign.deSign.Deserialize(verificaMarcaResponse.Esito.DatiGeneraliVerifica);
                    }
                    else if (verificaMarcaResponse.Warning != null)
                    {
                        datiGeneraliVerifica = DeSign.deSign.Deserialize(verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica);
                    }

                    if(datiGeneraliVerifica != null && datiGeneraliVerifica.timeStamp.Count > 0)
                    {
                        List<TimestampDocEntity> timestampDocEntities = new List<TimestampDocEntity>();
                        foreach(var timestamp in datiGeneraliVerifica.timeStamp)
                        {
                            var outTSR = new OutputResponseMarca();

                            var certificato = timestamp.certificate.Replace("-----BEGIN CERTIFICATE-----", string.Empty).Replace("-----END CERTIFICATE-----", string.Empty).Replace("\n", string.Empty);
                            var certBytes = Encoding.UTF8.GetBytes(certificato);
                            var cert = new X509Certificate2(Convert.FromBase64String(certificato));

                            if (DateTime.Now.CompareTo(cert.NotAfter.ToLocalTime()) > 0)
                                outTSR.descrizioneErrore = Resource.ElapsedTimestamp;

                            outTSR.dsm = cert.NotAfter.AsDateTimeFormat();
                            outTSR.sernum = timestamp.timeStampSerial;
                            outTSR.docm_date = !string.IsNullOrEmpty(timestamp.timeStampDate) ? DateTime.ParseExact(timestamp.timeStampDate, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).AsDateTimeFormat() : string.Empty;
                            outTSR.marca = certificato;
                            outTSR.snCertificato = cert.SerialNumber;
                            outTSR.TSA = new TSARFC2253()
                            {
                                TSARFC2253Name = String.Format("CN={0},OU={1},O={2},C={3}",
                                    timestamp.issuer.CN,
                                    timestamp.issuer.OU,
                                    timestamp.issuer.O,
                                    timestamp.issuer.C),
                                C = timestamp.issuer.C,
                                CN = timestamp.issuer.CN,
                                O = timestamp.issuer.O,
                                OU = timestamp.issuer.OU
                            };

                            System.Security.Cryptography.Oid oidHash = new System.Security.Cryptography.Oid(timestamp.timeStampImprintAlgorithm);
                            outTSR.algHash = oidHash.FriendlyName;
                            outTSR.esito = "OK";

                            var tsEntityToInsert = new TimestampDocEntity()
                            {
                                DOC_NUMBER = request.fileRequest.docNumber.AsLong(),
                                VERSION_ID = request.fileRequest.versionId.AsLong(),
                                ID_PEOPLE = idUser,
                                DTA_CREAZIONE = outTSR.docm_date.AsDateTime(),
                                DTA_SCADENZA = outTSR.dsm.AsDateTime(),
                                NUM_SERIE = outTSR.sernum,
                                S_N_CERTIFICATO = outTSR.snCertificato,
                                ALG_HASH = outTSR.algHash,
                                SOGGETTO = outTSR.TSA.O,
                                PAESE = outTSR.TSA.C,
                                TSR_FILE = outTSR.marca
                            };

                            timestampDocEntities.Add(tsEntityToInsert);
                        }

                        await _pi3DbContext.TimestampDocEntities.AddRangeAsync(timestampDocEntities);

                        await ((Pi3DbContext)this._pi3DbContext).SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                output = false;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new DocumentSaveTimestampResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentSaveTimestampHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;

        #endregion
    }

}
