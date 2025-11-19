// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
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
using GetElementiInLibroFirmaByDestinatarioRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetElementiInLibroFirmaByDestinatario;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetElementiInLibroFirmaByDestinatario
{
    public class GetElementiInLibroFirmaByDestinatarioHandler : IRequestHandler<GetElementiInLibroFirmaByDestinatarioRequest, GetElementiInLibroFirmaByDestinatarioResult>
    {
        #region Public Members

        public GetElementiInLibroFirmaByDestinatarioHandler(ILogger<GetElementiInLibroFirmaByDestinatarioHandler> logger,
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

        public async Task<GetElementiInLibroFirmaByDestinatarioResult> Handle(GetElementiInLibroFirmaByDestinatarioRequest request, CancellationToken cancellationToken)
        {
            string[] output = null;
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                long idCorrGlobali = 0;
                if (!string.IsNullOrEmpty(request.corr.systemId))
                {
                    idCorrGlobali = request.corr.systemId.AsLong();
                }
                else if (!string.IsNullOrEmpty(request.corr.codiceRubrica))
                {
                    idCorrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.VAR_COD_RUBRICA.ToUpper().Equals(request.corr.codiceRubrica.ToUpper()))
                        .Select(c => c.SYSTEM_ID)
                        .FirstOrDefaultAsync();
                }
                else if (!string.IsNullOrEmpty(request.corr.descrizione))
                {
                    idCorrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.VAR_DESC_CORR.ToUpper().Contains(request.corr.descrizione.ToUpper()))
                        .Select(c => c.SYSTEM_ID)
                        .FirstOrDefaultAsync();
                }

                output = await this._dbContext.ElementoInLibroFirmaEntities
                    .Join(this._dbContext.DocArrivoParEntities, elemento => elemento.DOC_NUMBER, docArrivoPar => docArrivoPar.ID_PROFILE, (elemento, docArrivoPar) => new { elemento, docArrivoPar })
                    .Where(j => j.elemento.ID_RUOLO_TITOLARE == idGruppo
                    && (j.elemento.ID_UTENTE_TITOLARE == null || j.elemento.ID_UTENTE_TITOLARE == idPeople)
                    && (j.elemento.ID_UTENTE_LOCKER == null || j.elemento.ID_UTENTE_LOCKER == idPeople)
                    && j.docArrivoPar.ID_MITT_DEST == idCorrGlobali)
                    .Select(j => j.elemento.ID_ELEMENTO.ToString())
                    .AsNoTracking()
                    .ToArrayAsync();

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new GetElementiInLibroFirmaByDestinatarioResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetElementiInLibroFirmaByDestinatarioHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ElementoInLibroFirmaEntity, ElementoInLibroFirma>()
                    .ForMember(dest => dest.IdElemento, src => src.MapFrom(opt => opt.ID_ELEMENTO))
                    .ForMember(dest => dest.StatoFirma, src => src.MapFrom(opt => (TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), opt.STATO_FIRMA)))
                    .ForMember(dest => dest.TipoFirma, src => src.MapFrom(opt => opt.TIPO_FIRMA))
                    .ForMember(dest => dest.Modalita, src => src.MapFrom(opt => opt.MODALITA))
                    .ForMember(dest => dest.DataInserimento, src => src.MapFrom(opt => opt.DATA_INSERIMENTO.AsDateTimeFormat()))
                    .ForMember(dest => dest.DataScadenza, src => src.MapFrom(opt => opt.SCADENZA.AsDateTimeFormat()))
                    .ForMember(dest => dest.DataAccettazione, src => src.MapFrom(opt => opt.DTA_ACCETTAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.IdRuoloTitolare, src => src.MapFrom(opt => opt.ID_RUOLO_TITOLARE))
                    .ForMember(dest => dest.DescProponenteDelegato, src => src.MapFrom(opt => opt.ID_PEOPLE_PROPONENTE_DELEGATO))
                    .ForMember(dest => dest.IdUtenteTitolare, src => src.MapFrom(opt => opt.ID_UTENTE_TITOLARE))
                    .ForMember(dest => dest.IdUtenteLocker, src => src.MapFrom(opt => opt.ID_UTENTE_LOCKER))
                    .ForMember(dest => dest.Note, src => src.MapFrom(opt => opt.NOTE))
                    .ForMember(dest => dest.IdIstanzaProcesso, src => src.MapFrom(opt => opt.ISTANZA_PROCESSO))
                    .ForMember(dest => dest.IdIstanzaPasso, src => src.MapFrom(opt => opt.ID_ISTANZA_PASSO))
                    .ForMember(dest => dest.IdTrasmSingola, src => src.MapFrom(opt => opt.ID_TRASM_SINGOLA))
                    .ForMember(dest => dest.ErroreFirma, src => src.MapFrom(opt => opt.ERRORE_FIRMA != null ? opt.ERRORE_FIRMA : string.Empty));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
