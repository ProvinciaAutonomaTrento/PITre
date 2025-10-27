// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerificaValiditaFirmaAllaDataRequest = Pi3.App.Legacy.WebApi.Application.Requests.VerificaValiditaFirmaAllaData;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.VerificaValiditaFirmaAllaData
{
    public class VerificaValiditaFirmaAllaDataHandler : IRequestHandler<VerificaValiditaFirmaAllaDataRequest, VerificaValiditaFirmaAllaDataResult>
    {
        #region Public Members

        public VerificaValiditaFirmaAllaDataHandler(ILogger<VerificaValiditaFirmaAllaDataHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IFirmaDigitale2Service firmaDigitale2Service)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._firmaDigitale2Service = firmaDigitale2Service;

            this.InitializeMapper();
        }

        public async Task<VerificaValiditaFirmaAllaDataResult> Handle(VerificaValiditaFirmaAllaDataRequest request, CancellationToken cancellationToken)
        {
            var fileDocumento = (await this._mediator.Send(new Requests.GetFileDocument(request.fileRequest, request.infoUtente))).output;

            var verificaFirmaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = fileDocumento.fullName.ToUpper().EndsWith("P7M"),
                FileFirmato = fileDocumento.content,
                DataVerifica = request.dataDiVerifica,
                TipoVerifica = TipiVerifica.Appiattita,
                ReturnFileOriginale = false,
                ReturnXmlCompleto = true
            });

            fileDocumento.signatureResult = new VerifySignatureResult();
            fileDocumento.signatureResult.FinalDocumentName = fileDocumento.nomeOriginale;
            fileDocumento.signatureResult.PKCS7Documents = new PKCS7Document[1];
            fileDocumento.signatureResult.PKCS7Documents[0] = new PKCS7Document();
            if (verificaFirmaResponse.Esito != null)
            {
                fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.Valid;
                fileDocumento.signatureResult.PKCS7Documents[0].SignersInfo = _mapper.Map<SignerInfo[]>(verificaFirmaResponse.Esito.DatiFirmatari);
            }
            else if (verificaFirmaResponse.Warning != null)
            {
                fileDocumento.signatureResult.PKCS7Documents[0].SignersInfo = _mapper.Map<SignerInfo[]>(verificaFirmaResponse.Warning.DettaglioFirmaDigitale.DatiFirmatari);
                fileDocumento.signatureResult.ErrorMessages = verificaFirmaResponse.Warning.WarningFault.Select(w => w.ErrorMsg).ToArray();
                fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.ErroreGenerico;
                verificaFirmaResponse.Warning.WarningFault.ForEach(w =>
                {
                    switch (w.ErrorCode)
                    {
                        case "1426":
                            fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.CtlNotTimeValid;
                            break;
                        case "1468":
                            fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.SHA1NonSupportato;
                            break;
                        case "1407":
                            fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.NotTimeValid;
                            break;
                        case "1408":
                            fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.Revoked;
                            break;
                    }
                });
            }

            //INIZIO processFileInformationCRLUpdate
            var versionId = request.fileRequest.versionId.AsLong();
            var docnumber = request.fileRequest.docNumber.AsLong();

            var componentEntity = await _dbContext.ComponentEntities
                .Where(c => c.VERSION_ID == versionId && c.DOCNUMBER == docnumber)
                .FirstAsync();

            FileInformation fileInfo = FileInformation.decodeMask(componentEntity.FILE_INFO);
            //controllo gi� effettuato in precedenza, non lo rifaccio..
            if ((fileInfo.Signature != FileInformation.VerifyStatus.Valid) ||
                (fileInfo.CrlStatus != FileInformation.VerifyStatus.Valid))
            {
                if (fileDocumento.signatureResult.StatusCode == -100)
                {
                    //server sta giu o non o funzionante
                    fileInfo.Signature = FileInformation.VerifyStatus.InProgress;
                    fileInfo.CrlStatus = FileInformation.VerifyStatus.InProgress;
                }
                else
                {
                    var hasErrs = fileDocumento.signatureResult.ErrorMessages != null && fileDocumento.signatureResult.ErrorMessages.Length != 0;
                    fileInfo.Signature = fileDocumento.signatureResult.StatusCode != -1 && !hasErrs ? FileInformation.VerifyStatus.Valid : FileInformation.VerifyStatus.Invalid;

                    //CONTROLLO CRL
                    fileInfo.CrlStatus = await RevokedCertArePresent(fileDocumento) ? FileInformation.VerifyStatus.Invalid : FileInformation.VerifyStatus.Valid;
                }

                fileInfo.setGlobalStatus();
                if (fileInfo.CheckRefDate == DateTime.MinValue)
                    fileInfo.CheckRefDate = DateTime.Now;
                fileInfo.CrlRefDate = request.dataDiVerifica;

                componentEntity.FILE_INFO = FileInformation.encodeMask(fileInfo);

                await ((DbContext)_dbContext).SaveChangesAsync();

                //FINE processFileInformationCRLUpdate
            }

            return new VerificaValiditaFirmaAllaDataResult(fileDocumento);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<VerificaValiditaFirmaAllaDataHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;

        protected enum EsitoVerificaStatus
        {
            Valid = 0,         //OK
            NotTimeValid = 1,  //Scaduto
            Revoked = 4,       //Revocato
            CtlNotTimeValid = 131072, //Data non corretta
            ErroreGenerico = -1,
            SHA1NonSupportato = -2
        }

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DatiFirmatari, SignerInfo>()
                     .ForMember(dest => dest.SignatureAlgorithm, opt => opt.MapFrom(src => src.Firmatario.DigestAlgorithm))
                     .ForMember(dest => dest.SigningTime, opt => opt.MapFrom(src => src.Firmatario.DataOraFirma))
                     .AfterMap((src, dest) =>
                     {
                         dest.SubjectInfo = new SubjectInfo()
                         {
                             CertId = src.Firmatario.DistinguishName,
                             Cognome = src.Firmatario.Cognome,
                             Nome = src.Firmatario.Nome,
                             CodiceFiscale = src.Firmatario.CodiceFiscale,
                             Country = src.Firmatario.Nazione
                         };

                         dest.CertificateInfo = new CertificateInfo()
                         {
                             IssuerName = $"CN={src.Firmatario.CnCertAuthority}",
                             SerialNumber = src.Firmatario.SerialNumber,
                             SubjectName = src.Firmatario.CommonName,
                             ValidFromDate = src.Firmatario.DataInizioValiditaCert,
                             ValidToDate = src.Firmatario.DataFineValiditaCert,
                         };

                         if(src.MarcaFirma != null)
                         {
                             dest.SignatureTimeStampInfo = new TSInfo[1]
                             {
                                 new TSInfo()
                                 {
                                     TSANameIssuer = src.MarcaFirma.TSANameIssuer,
                                     TSANameSubject = src.MarcaFirma.TSANameSubject,
                                     TSdateTime = src.MarcaFirma.TSdateTime,
                                     TSimprint = src.MarcaFirma.TSimprint,
                                     TSserialNumber = src.MarcaFirma.TSserialNumber,
                                     dataFineValiditaCert = src.MarcaFirma.DataFineValiditaCert,
                                     dataInizioValiditaCert = src.MarcaFirma.DataInizioValiditaCert
                                 }
                             };
                         }
                     });
            });

            this._mapper = configuration.CreateMapper();
        }

        protected virtual async Task<bool> RevokedCertArePresent(FileDocumento filedoc)
        {
            foreach (PKCS7Document p7md in filedoc.signatureResult.PKCS7Documents)
            {
                foreach (SignerInfo siinfo in p7md.SignersInfo)
                {
                    if (siinfo.CertificateInfo.RevocationDate != DateTime.MinValue)
                        return true;
                }
            }
            return false;
        }

        #endregion
    }
}