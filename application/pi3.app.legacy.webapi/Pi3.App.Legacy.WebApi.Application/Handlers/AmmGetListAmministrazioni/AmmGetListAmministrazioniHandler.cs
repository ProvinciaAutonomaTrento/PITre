// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.SmartClient;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetListAmministrazioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListAmministrazioni;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListAmministrazioni
{
    public class AmmGetListAmministrazioniHandler : IRequestHandler<AmmGetListAmministrazioniRequest, AmmGetListAmministrazioniResult>
    {
        #region Public Members

        public AmmGetListAmministrazioniHandler(ILogger<AmmGetListAmministrazioniHandler> logger,
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

        public async Task<AmmGetListAmministrazioniResult> Handle(AmmGetListAmministrazioniRequest request, CancellationToken cancellationToken)
        {
            InfoAmministrazione[] output = null;

            try
            {
                var amministraEntities = await this._dbContext.AmministraEntities
                    .OrderBy(a => a.VAR_DESC_AMM)
                    .AsNoTracking()
                    .ToListAsync();

                var caratTimbroEntity = await this._dbContext.CaratTimbroEntities
                    .AsNoTracking()
                    .OrderBy(c => c.SYSTEM_ID)
                    .ToListAsync();

                var coloreTimbroEntity = await this._dbContext.ColoreTimbroEntities
                    .AsNoTracking()
                    .OrderBy(c => c.SYSTEM_ID)
                    .ToListAsync();

                var posizioneTimbroEntity = await this._dbContext.PosizTimbroEntities
                    .AsNoTracking()
                    .OrderBy(c => c.SYSTEM_ID)
                    .ToListAsync();

                InfoTimbro timbro = new InfoTimbro();
                timbro.carattere = this._mapper.Map<carattere[]>(caratTimbroEntity);
                timbro.color = this._mapper.Map<color[]>(coloreTimbroEntity);
                timbro.positions = this._mapper.Map<posizione[]>(posizioneTimbroEntity);

                if (amministraEntities != null && amministraEntities.Any())
                {
                    output = this._mapper.Map<InfoAmministrazione[]>(amministraEntities);
                    foreach (var item in output)
                    {
                        item.Timbro = timbro;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new AmmGetListAmministrazioniResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListAmministrazioniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AmministrazioneEntity, InfoAmministrazione>()
                    .ForMember(dest => dest.IDAmm, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_AMM))
                    .ForMember(dest => dest.LibreriaDB, src => src.MapFrom(opt => opt.VAR_LIBRERIA))
                    .ForMember(dest => dest.Segnatura, src => src.MapFrom(opt => opt.VAR_FORMATO_SEGNATURA))
                    .ForMember(dest => dest.Fascicolatura, src => src.MapFrom(opt => opt.VAR_FORMATO_FASCICOLATURA))
                    .ForMember(dest => dest.DettaglioFirma, src => src.MapFrom(opt => opt.VAR_DETTAGLIO_FIRMA))
                    .ForMember(dest => dest.Dominio, src => src.MapFrom(opt => opt.VAR_DOMINIO))
                    .ForMember(dest => dest.formatoDominio, src => src.MapFrom(opt => opt.VAR_FORMATO_DOMINIO))
                    .ForMember(dest => dest.ServerSMTP, src => src.MapFrom(opt => opt.VAR_SMTP))
                    .ForMember(dest => dest.PortaSMTP, src => src.MapFrom(opt => opt.NUM_PORTA_SMTP))
                    .ForMember(dest => dest.UserSMTP, src => src.MapFrom(opt => opt.VAR_USER_SMTP))
                    .ForMember(dest => dest.PasswordSMTP, src => src.MapFrom(opt => opt.VAR_PWD_SMTP))
                    .ForMember(dest => dest.IDRagioneTO, src => src.MapFrom(opt => opt.ID_RAGIONE_TO))
                    .ForMember(dest => dest.IDRagioneCC, src => src.MapFrom(opt => opt.ID_RAGIONE_CC))
                    .ForMember(dest => dest.GGPermanenzaTDL, src => src.MapFrom(opt => opt.NUM_GG_PERM_TODOLIST))
                    .ForMember(dest => dest.AttivaGGPermanenzaTDL, src => src.MapFrom(opt => opt.CHA_ATTIVA_GG_PERM_TODOLIST))
                    .ForMember(dest => dest.SslSMTP, src => src.MapFrom(opt => opt.CHA_SMTP_SSL))
                    .ForMember(dest => dest.StaSMTP, src => src.MapFrom(opt => opt.CHA_SMTP_STA))
                    .ForMember(dest => dest.FromEmail, src => src.MapFrom(opt => opt.FROM_EMAIL_ADDRESS))
                    .ForMember(dest => dest.IDRagioneCompetenza, src => src.MapFrom(opt => opt.ID_RAGIONE_COMPETENZA))
                    .ForMember(dest => dest.IDRagioneConoscenza, src => src.MapFrom(opt => opt.ID_RAGIONE_CONOSCENZA))
                    .ForMember(dest => dest.Timbro_pdf, src => src.MapFrom(opt => opt.VAR_FORMATO_TIMBRO))
                    .ForMember(dest => dest.Timbro_orientamento, src => src.MapFrom(opt => opt.ORIENTAMENTO))
                    .ForMember(dest => dest.Timbro_carattere, src => src.MapFrom(opt => (opt.ID_CARAT_DF == 0 || opt.ID_CARAT_DF == null) ? "1" : opt.ID_CARAT_DF.ToString()))
                    .ForMember(dest => dest.Timbro_colore, src => src.MapFrom(opt => (opt.ID_COLORE_DF == 0 || opt.ID_COLORE_DF == null) ? "1" : opt.ID_COLORE_DF.ToString()))
                    .ForMember(dest => dest.Timbro_posizione, src => src.MapFrom(opt => (opt.ID_POS_DF == 0 || opt.ID_POS_DF == null) ? "1" : opt.ID_POS_DF.ToString()))
                    .ForMember(dest => dest.Timbro_rotazione, src => src.MapFrom(opt => opt.TIPO_ROTAZ))
                    .ForMember(dest => dest.Banner, src => src.MapFrom(opt => opt.VAR_MSG_BANNER))
                    .ForMember(dest => dest.IdClientSideModelProcessor, src => src.MapFrom(opt => opt.ID_CLIENT_MODEL_PROCESSOR))
                    .ForMember(dest => dest.formatoProtTitolario, src => src.MapFrom(opt => opt.VAR_FORMATO_PROT_TIT))
                    .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_AMM_IPA))
                    .ForMember(dest => dest.indirizzoDigitaleRiferimento, src => src.MapFrom(opt => opt.VAR_INDIRIZZO_DIGITALE_RIF))
                    .ForMember(dest => dest.TipologiaDocumentoObbligatoria, src => src.MapFrom(opt => opt.TIPO_DOC_OBBL))
                    .ForMember(dest => dest.DispositivoStampa, src => src.MapFrom(opt => opt.ID_DISPOSITIVO_STAMPA))
                    .AfterMap((src, dest) =>
                    {
                        dest.SpedizioneDocumenti = new DocsPaVO.Spedizione.ConfigSpedizioneDocumento()
                        {
                            SpedizioneAutomaticaDocumento = src.SPEDIZIONE_AUTO_DOC == "1",
                            TrasmissioneAutomaticaDocumento = src.TRASMISSIONE_AUTO_DOC == "1",
                            AvvisaSuSpedizioneDocumento = src.AVVISA_SPEDIZIONE_DOC == "1"
                        };

                        dest.SmartClientConfigurations = new SmartClientConfigurations()
                        {
                            ComponentsType = src.CHA_TIPO_COMPONENTI,
                            ApplyPdfConvertionOnScan = src.CHA_TIPO_COMPONENTI != "1" && src.CHA_TIPO_COMPONENTI != "0" && src.SMART_CLIENT_PDF_CONV_ON_SCAN == "1"
                        };
                    });

                cfg.CreateMap<CaratTimbroEntity, carattere>()
                    .ForMember(dest => dest.id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.caratName, src => src.MapFrom(opt => opt.VAR_NOME))
                    .ForMember(dest => dest.dimensione, src => src.MapFrom(opt => opt.DIMENSIONE));

                cfg.CreateMap<ColoreTimbroEntity, color>()
                    .ForMember(dest => dest.id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.colName, src => src.MapFrom(opt => opt.VAR_NOME))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIZIONE));

                cfg.CreateMap<PosizTimbroEntity, posizione>()
                    .ForMember(dest => dest.id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.posName, src => src.MapFrom(opt => opt.TIPO_POS))
                    .ForMember(dest => dest.PosX, src => src.MapFrom(opt => opt.POS_X))
                    .ForMember(dest => dest.PosX, src => src.MapFrom(opt => opt.POS_Y));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
