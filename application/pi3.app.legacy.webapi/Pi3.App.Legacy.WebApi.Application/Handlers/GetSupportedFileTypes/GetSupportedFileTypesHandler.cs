// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.FormatiDocumento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetSupportedFileTypesRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetSupportedFileTypes;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetSupportedFileTypes
{
    public class GetSupportedFileTypesHandler : IRequestHandler<GetSupportedFileTypesRequest, GetSupportedFileTypesResult>
    {
        #region Public Members

        public GetSupportedFileTypesHandler(ILogger<GetSupportedFileTypesHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetSupportedFileTypesResult> Handle(GetSupportedFileTypesRequest request, CancellationToken cancellationToken)
        {
            //In PiTre la chiave SUPPORTED_FILE_TYPES_ENABLED è sempre abilita per cui evito il controllo
            SupportedFileType[] output = null!;

            try
            {
                var idAmministrazione = Convert.ToInt64(request.idAmministrazione);

                var supportedFileTypesEntities = 
                        await this._dbContext.FormatoDocumentoEntities
                             .Join(this._dbContext.AmministraEntities,
                                formato => formato.ID_AMMINISTRAZIONE, amm => amm.SYSTEM_ID, (formato, amm) => new { formato, amm })
                             .Where(j => j.amm.SYSTEM_ID == idAmministrazione)
                             .Select(j => new SupportedFileTypeEntity
                             {
                                 FORMATO_DOCUMENTO = j.formato,
                                 CODICE_AMMINISTRAZIONE = j.amm.VAR_CODICE_AMM
                             })
                             .OrderBy(j => j.FORMATO_DOCUMENTO.DESCRIPTION)
                             .AsNoTracking()
                             .ToListAsync();

                output = this._mapper.Map<SupportedFileType[]>(supportedFileTypesEntities);
            }
            catch (Pi3Exception pi3Ex)
            {
                _logger.LogError(pi3Ex, null!, null!);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, null!, null!);
            }

            return new GetSupportedFileTypesResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSupportedFileTypesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<SupportedFileTypeEntity, SupportedFileType>()
                    .ForMember(dest => dest.SystemId, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.SYSTEM_ID))
                    .ForMember(dest => dest.IdAmministrazione, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.ID_AMMINISTRAZIONE))
                    .ForMember(dest => dest.CodiceAmministrazione, opt => opt.MapFrom(src => src.CODICE_AMMINISTRAZIONE))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.DESCRIPTION))
                    .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.FILE_EXTENSION))
                    .ForMember(dest => dest.MaxFileSize, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.MAX_FILE_SIZE))
                    .ForMember(dest => dest.MaxFileSizeAlertMode, opt => opt.MapFrom(src => (MaxFileSizeAlertModeEnum)Convert.ToInt32(src.FORMATO_DOCUMENTO.MAX_FILE_SIZE_ALERT_MODE)))
                    .ForMember(dest => dest.ContainsFileModel, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.CONTAINS_FILE_MODEL > 0))
                    .ForMember(dest => dest.FileTypeUsed, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.FILE_TYPE_USED > 0))
                    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => (DocumentTypeEnum)Convert.ToInt32(src.FORMATO_DOCUMENTO.DOCUMENT_TYPE)))
                    .ForMember(dest => dest.FileTypePreservation, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.FILE_TYPE_PRESERVATION > 0))
                    .ForMember(dest => dest.FileTypeSignature, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.FILE_TYPE_SIGNATURE > 0))
                    .ForMember(dest => dest.FileTypeValidation, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.FILE_TYPE_VALIDATION > 0))
                    .ForMember(dest => dest.FileTypeConvertible, opt => opt.MapFrom(src => src.FORMATO_DOCUMENTO.CHA_CONVERTIBLE == "1" ? true : false));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class SupportedFileTypeEntity
        {
            public FormatoDocumentoEntity FORMATO_DOCUMENTO { get; set; }

            public string? CODICE_AMMINISTRAZIONE { get; set; }
        }

        #endregion
    }

}
