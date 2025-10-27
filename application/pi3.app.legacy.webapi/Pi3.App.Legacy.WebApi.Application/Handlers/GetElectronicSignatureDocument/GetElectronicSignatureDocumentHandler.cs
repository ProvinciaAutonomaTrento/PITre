// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
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
using System.Xml;
using GetElectronicSignatureDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetElectronicSignatureDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetElectronicSignatureDocument
{

    public class GetElectronicSignatureDocumentHandler : IRequestHandler<GetElectronicSignatureDocumentRequest, GetElectronicSignatureDocumentResult>
    {
        #region Public Members

        public GetElectronicSignatureDocumentHandler(ILogger<GetElectronicSignatureDocumentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetElectronicSignatureDocumentResult> Handle(GetElectronicSignatureDocumentRequest request, CancellationToken cancellationToken)
        {
            List<FirmaElettronica> output = new List<FirmaElettronica>();

            try
            {
                if (!string.IsNullOrEmpty(request.docnumber))
                {
                    var docnumber = request.docnumber.AsLong();
                    var versionId = request.versionId.AsLong();

                    var firmaElettronicaEntities = await this._dbContext.FirmaElettronicaEntities.AsNoTracking()
                        .Where(f => f.ID_DOCUMENTO == docnumber && f.VERSION_ID == versionId && f.XML != null)
                        .OrderBy(f => f.DATA_APPOSIZIONE)
                        .ToListAsync();

                    output = this._mapper.Map<FirmaElettronica[]>(firmaElettronicaEntities).ToList();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetElectronicSignatureDocumentResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetElectronicSignatureDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FirmaElettronicaEntity, FirmaElettronica>()
                     .ForMember(dest => dest.IdFirma, opt => opt.MapFrom(src => src.ID_FIRMA))
                     .ForMember(dest => dest.Docnumber, opt => opt.MapFrom(src => src.ID_DOCUMENTO))
                     .ForMember(dest => dest.Versionid, opt => opt.MapFrom(src => src.VERSION_ID))
                     .ForMember(dest => dest.Xml, opt => opt.MapFrom(src => src.XML ?? string.Empty))
                     .AfterMap((src, dest) =>
                     {
                         if(!string.IsNullOrEmpty(src.XML))
                         {
                             XmlDocument doc = new XmlDocument();
                             XmlTextReader xtr = new XmlTextReader(new System.IO.StringReader(src.XML));
                             doc.Load(xtr);

                             XmlElement elFirmatario = (XmlElement)doc.DocumentElement.SelectSingleNode("FirmaElettronica/Firmatario");
                             string ruoloFirmatario = elFirmatario.SelectSingleNode("Ruolo").InnerText.Trim();
                             string utenteFirmatario = elFirmatario.SelectSingleNode("Utente").InnerText.Trim();

                             XmlElement elDataCreazione = (XmlElement)doc.DocumentElement.SelectSingleNode("FirmaElettronica/DataCreazione");
                             dest.DataApposizione = elDataCreazione.InnerText.Trim();

                             string delegato = elFirmatario.GetAttribute("delega");
                             dest.Firmatario = string.IsNullOrEmpty(delegato) ? $"{utenteFirmatario} ({ruoloFirmatario})" : $"{delegato} ({ruoloFirmatario}) {Resources.SostituoDi} {utenteFirmatario}";
                         }
                     });
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
