// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.AmmGetInfoAmmCorrente
{

    // Richiede libreria MediatR
    public class AmmGetInfoAmmCorrenteHandler : IRequestHandler<Application.Requests.AmmGetInfoAmmCorrente, AmmGetInfoAmmCorrenteResult>
    {
        #region Public Members

        public AmmGetInfoAmmCorrenteHandler(ILogger<AmmGetInfoAmmCorrenteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<AmmGetInfoAmmCorrenteResult> Handle(Application.Requests.AmmGetInfoAmmCorrente request, CancellationToken cancellationToken)
        {
            InfoAmministrazione infoAmministrazione = null!;

            try
            {
                var idAmm = request.idAmm.AsLong();

                var amministraEntity = await this._dbContext.AmministraEntities
                    .AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idAmm)
                    .FirstAsync();

                infoAmministrazione = _mapper.Map<InfoAmministrazione>(amministraEntity);

                var timbro = new InfoTimbro();

                //Carattere
                var infoCarattereTimbro = _dbContext.CaratTimbroEntities
                    .AsNoTracking()
                    .OrderBy(x => x.SYSTEM_ID)
                    .ToList();

                var caratteriList = new List<carattere>();

                infoCarattereTimbro.ForEach(x => 
                    caratteriList.Add(new carattere
                    {
                        id = x.SYSTEM_ID.ToString(),
                        caratName = x.VAR_NOME,
                        dimensione = x.DIMENSIONE
                    }));
                timbro.carattere = caratteriList.ToArray();

                //Colore
                var infoColoreTimbro = _dbContext.ColoreTimbroEntities
                    .AsNoTracking()
                    .OrderBy(x => x.SYSTEM_ID)
                    .ToList();

                var colorList = new List<color>();
                infoColoreTimbro.ForEach(x => colorList.Add(new color
                {
                    id = x.SYSTEM_ID.ToString(),
                    colName = x.VAR_NOME,
                    descrizione = x.DESCRIZIONE
                }));
                timbro.color = colorList.ToArray();

                //Posizione
                var infoPosizioneTimbro = _dbContext.PosizTimbroEntities
                    .AsNoTracking()
                    .OrderBy(x => x.SYSTEM_ID)
                    .ToList();

                var positionList = new List<posizione>();
                infoPosizioneTimbro.ForEach(x => positionList.Add(new posizione
                {
                    id = x.SYSTEM_ID.ToString(),
                    posName = x.TIPO_POS,
                    PosX = x.POS_X,
                    PosY = x.POS_Y
                }));
                timbro.positions = positionList.ToArray();

                infoAmministrazione.Timbro = timbro;
                
                //SpedizioneDocumenti
                infoAmministrazione.SpedizioneDocumenti = new DocsPaVO.Spedizione.ConfigSpedizioneDocumento
                {
                    SpedizioneAutomaticaDocumento = amministraEntity.SPEDIZIONE_AUTO_DOC.Equals("1"),
                    TrasmissioneAutomaticaDocumento = amministraEntity.TRASMISSIONE_AUTO_DOC.Equals("1"),
                    AvvisaSuSpedizioneDocumento = amministraEntity.AVVISA_SPEDIZIONE_DOC.Equals("1")
                };

                //SmartClientConfigurations
                infoAmministrazione.SmartClientConfigurations = new DocsPaVO.SmartClient.SmartClientConfigurations
                {
                    ApplyPdfConvertionOnScan = !amministraEntity.CHA_TIPO_COMPONENTI.Equals("1") && !amministraEntity.CHA_TIPO_COMPONENTI.Equals("0") && amministraEntity.SMART_CLIENT_PDF_CONV_ON_SCAN.Equals("1"),
                    ComponentsType = amministraEntity.CHA_TIPO_COMPONENTI
                };
            }
            catch (Pi3Exception pi3Ex)
            {
                _logger.LogError(pi3Ex, null!, null!);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, null!, null!);
            }

            return new AmmGetInfoAmmCorrenteResult(infoAmministrazione);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetInfoAmmCorrenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null!;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AmministrazioneEntity, InfoAmministrazione>()
                    .ForMember(dest => dest.IDAmm, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_AMM))
                    .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_AMM_IPA))
                    .ForMember(dest => dest.indirizzoDigitaleRiferimento, src => src.MapFrom(opt => opt.VAR_INDIRIZZO_DIGITALE_RIF))
                    .ForMember(dest => dest.LibreriaDB, src => src.MapFrom(opt => opt.VAR_LIBRERIA))
                    .ForMember(dest => dest.Segnatura, src => src.MapFrom(opt => opt.VAR_FORMATO_SEGNATURA))
                    .ForMember(dest => dest.Fascicolatura, src => src.MapFrom(opt => opt.VAR_FORMATO_FASCICOLATURA))
                    .ForMember(dest => dest.Dominio, src => src.MapFrom(opt => opt.VAR_DOMINIO))
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
                    .ForMember(dest => dest.IDRagioneCompetenza, src => src.MapFrom(opt => opt.ID_RAGIONE_CONOSCENZA))
                    .ForMember(dest => dest.IDRagioneConoscenza, src => src.MapFrom(opt => opt.ID_RAGIONE_CONOSCENZA))
                    .ForMember(dest => dest.Timbro_pdf, src => src.MapFrom(opt => opt.VAR_FORMATO_TIMBRO))
                    .ForMember(dest => dest.DettaglioFirma, src => src.MapFrom(opt => opt.VAR_DETTAGLIO_FIRMA))
                    .ForMember(dest => dest.Timbro_orientamento, src => src.MapFrom(opt => opt.ORIENTAMENTO))
                    .ForMember(dest => dest.Timbro_carattere, src => src.MapFrom(opt => opt.ID_CARAT_DF == null || opt.ID_CARAT_DF == 0 ? 1 : opt.ID_CARAT_DF))
                    .ForMember(dest => dest.Timbro_colore, src => src.MapFrom(opt => opt.ID_COLORE_DF == null || opt.ID_COLORE_DF == 0 ? 1 : opt.ID_COLORE_DF))
                    .ForMember(dest => dest.Timbro_posizione, src => src.MapFrom(opt => opt.ID_POS_DF == null || opt.ID_POS_DF == 0 ? 1 : opt.ID_POS_DF))
                    .ForMember(dest => dest.Timbro_rotazione, src => src.MapFrom(opt => opt.TIPO_ROTAZ))
                    .ForMember(dest => dest.Banner, src => src.MapFrom(opt => opt.VAR_MSG_BANNER))
                    .ForMember(dest => dest.IdClientSideModelProcessor, src => src.MapFrom(opt => opt.ID_CLIENT_MODEL_PROCESSOR))
                    .ForMember(dest => dest.formatoProtTitolario, src => src.MapFrom(opt => opt.VAR_FORMATO_PROT_TIT))
                    .ForMember(dest => dest.TipologiaDocumentoObbligatoria, src => src.MapFrom(opt => opt.TIPO_DOC_OBBL));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
