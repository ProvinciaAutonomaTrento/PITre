// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.Modelli_Trasmissioni;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.UtentiConNotificaTrasm;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByID
{
    public class GetModelloByIDCommandHandler : IRequestHandler<GetModelloByIDCommand, GetModelloByIDCommandResponse>
    {
        public GetModelloByIDCommandHandler(
            ILogger<GetModelloByIDCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<GetModelloByIDCommandResponse> Handle(GetModelloByIDCommand request, CancellationToken cancellationToken)
        {

            ModelloTrasmissione output = null;

            try
            {
                var idAmministrazione = request.IdAmm.AsLong();
                var idModelloAsLong = request.IdModello.AsLong();

                var modelloTrasmEntities = await this._dbContext.ModelloTrasmEntities
                    .Where(m => m.ID_AMM == idAmministrazione && m.SYSTEM_ID == idModelloAsLong)
                    .OrderBy(m => m.ID_REGISTRO)
                    .ThenBy(m => m.SINGLE)
                    .ThenBy(m => m.NOME)
                    .FirstAsync();

                output = _mapper.Map<ModelloTrasmissione>(modelloTrasmEntities);

                if (output != null && output.SINGLE.Equals("0"))
                {
                    var modelloMittEntities_OLD = await this._dbContext.ModelloMittDestEntities
                        .Join(this._dbContext.CorrGlobaliEntities, modelloMittDest => modelloMittDest.ID_CORR_GLOBALI, corrGlobali => corrGlobali.SYSTEM_ID, (modelloMittDest, corrGlobali) => new { modelloMittDest, corrGlobali.VAR_COD_RUBRICA, corrGlobali.VAR_DESC_CORR })
                        .Where(m => m.modelloMittDest.ID_MODELLO == idModelloAsLong && m.modelloMittDest.CHA_TIPO_MITT_DEST == "M")
                        .ToListAsync();

                    var modelloMittEntities = await this._dbContext.ModelloMittDestEntities
                        .Where(m => m.ID_MODELLO == idModelloAsLong && m.CHA_TIPO_MITT_DEST == "M")
                        .Select(x => new MittDest
                        {
                            SYSTEM_ID = Convert.ToInt32(x.SYSTEM_ID),
                            ID_CORR_GLOBALI = Convert.ToInt32(x.ID_CORR_GLOBALI),
                            ID_MODELLO = Convert.ToInt32(x.ID_MODELLO),
                            CHA_TIPO_MITT_DEST = x.CHA_TIPO_MITT_DEST,
                            ID_RAGIONE = Convert.ToInt32(x.ID_RAGIONE),
                            CHA_TIPO_TRASM = x.CHA_TIPO_TRASM,
                            VAR_NOTE_SING = x.VAR_NOTE_SING,
                            CHA_TIPO_URP = x.CHA_TIPO_URP
                        })
                        .ToListAsync();

                    List<MittDest> mittDestList = new List<MittDest>();
                    foreach (var m in modelloMittEntities)
                    {
                        var mittDest = _mapper.Map<MittDest>(m);
                        mittDest.VAR_COD_RUBRICA = await GetCodRubricaCorr(m.ID_CORR_GLOBALI);
                        mittDest.DESCRIZIONE = await GetDescCorr(m.ID_CORR_GLOBALI);
                        mittDestList.Add(mittDest);
                    }
                    output.MITTENTE = mittDestList.ToArray();
                }

                string[] tipo_mitt_dest = new string[] { "D", "UT_P", "R_P", "RSP_P", "UO_P", "R_S", "RSP_M", "S_M" };
                var modelloDestEntities = await this._dbContext.ModelloMittDestEntities
                    .Join(this._dbContext.RagioneTrasmissioneEntities, dest => dest.ID_RAGIONE, ragione => ragione.SYSTEM_ID, (dest, ragione) => new { dest, ragione })
                    .GroupJoin(this._dbContext.CorrGlobaliEntities, dest => dest.dest.ID_CORR_GLOBALI, corr => corr.SYSTEM_ID, (dest, corr) => new { dest.dest, dest.ragione, corr })
                    .SelectMany(dest => dest.corr.DefaultIfEmpty(), (dest, corr) => new
                    {
                        dest.dest,
                        VAR_DESC_RAGIONE = dest.ragione.VAR_DESC_RAGIONE,
                        CHA_TIPO_RAGIONE = dest.ragione.CHA_TIPO_RAGIONE,
                        VAR_COD_RUBRICA = corr.VAR_COD_RUBRICA,
                        VAR_DESC_CORR = corr.VAR_DESC_CORR,
                        DTA_FINE = corr.DTA_FINE,
                        CHA_DISABLED_TRASM = corr.CHA_DISABLED_TRASM,

                    })
                    .Where(m => m.dest.ID_MODELLO == idModelloAsLong && tipo_mitt_dest.Contains(m.dest.CHA_TIPO_MITT_DEST))
                    .OrderBy(m => m.dest.ID_RAGIONE)
                    .ToListAsync();

                List<RagioneDest> ragioneDestList = new List<RagioneDest>();
                foreach (var d in modelloDestEntities)
                {

                    RagioneDest ragioneDest = null;

                    List<MittDest> destinatariList = new List<MittDest>();
                    var dest = _mapper.Map<MittDest>(d.dest);

                    dest.VAR_COD_RUBRICA = d.VAR_COD_RUBRICA;
                    dest.DESCRIZIONE = d.VAR_DESC_CORR;
                    dest.Disabled = d.DTA_FINE.HasValue;
                    if (d.dest.CHA_TIPO_URP != null && d.dest.CHA_TIPO_URP.Equals("R"))
                        dest.Inhibited = d.CHA_DISABLED_TRASM != null && d.CHA_DISABLED_TRASM.Equals("1");

                    if (dest.Inhibited)
                        continue;

                    if (ragioneDest == null || ragioneDest.RAGIONE != d.VAR_DESC_RAGIONE)
                    {
                        if (ragioneDest != null)
                        {
                            if (output.RAGIONI_DESTINATARI == null)
                            {
                                output.RAGIONI_DESTINATARI = new RagioneDest[1];
                                //output.RAGIONI_DESTINATARI[0] = new RagioneDest();
                            }
                            output.RAGIONI_DESTINATARI[0] = (ragioneDest);
                        }


                        ragioneDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                        ragioneDest.RAGIONE = d.VAR_DESC_RAGIONE;
                        ragioneDest.CHA_TIPO_RAGIONE = d.CHA_TIPO_RAGIONE;
                    }

                    if (dest.CHA_TIPO_MITT_DEST == "D" && dest.CHA_TIPO_URP == "R")
                    {
                        var utentiEntities = await this._dbContext.PeopleEntities
                            .Join(this._dbContext.PeopleGroupEntities, people => people.SYSTEM_ID, peopleGroup => peopleGroup.PEOPLE_SYSTEM_ID, (people, peoplegroups) => new { people, peoplegroups.DTA_FINE, peoplegroups.GROUPS_SYSTEM_ID })
                            .Join(this._dbContext.CorrGlobaliEntities, people => people.GROUPS_SYSTEM_ID, corr => corr.ID_GRUPPO, (people, corr) => new { people, corr.VAR_DESC_CORR, ID_CORR_GLOBALI = corr.SYSTEM_ID })
                            .Where(p => p.ID_CORR_GLOBALI == d.dest.ID_CORR_GLOBALI && p.people.DTA_FINE == null)
                            .OrderBy(p => p.people.people.VAR_COGNOME).ToListAsync();

                        List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm> utentiNotificaList = new List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm>();
                        utentiEntities.ForEach(u =>
                            utentiNotificaList.Add(new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm()
                            {
                                ID_PEOPLE = u.people.people.SYSTEM_ID.ToString(),
                                CODICE_UTENTE = u.people.people.USER_ID,
                                NOME_COGNOME_UTENTE = u.people.people.FULL_NAME,
                                ID_MODELLO_MITT_DEST = dest.SYSTEM_ID.ToString(),
                                FLAG_NOTIFICA = this._dbContext.ModelloDestConNotificaEntities.Any(n => n.ID_MODELLO == dest.ID_MODELLO && n.ID_PEOPLE == u.people.people.SYSTEM_ID) ? "1" : "0"
                            }));
                        dest.UTENTI_NOTIFICA = utentiNotificaList.ToArray();
                    }

                    destinatariList.Add(dest);
                    ragioneDest.DESTINATARI = destinatariList.ToArray();
                    ragioneDestList.Add(ragioneDest);
                }
                output.RAGIONI_DESTINATARI = ragioneDestList.ToArray();
                output = (await this._mediator.Send(new UtentiConNotificaTrasmCommand(){
                   ObjModTrasm = output,
                   UtentiDaCancellare = null,
                   UtentiDaInserire = null,
                   Operazione = "GET"
                })).Output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = output
            };
        }

        #region Private Members
        protected readonly ILogger<GetModelloByIDCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        private async Task<string> GetDescCorr(int idCorrGlobali)
        {
            return await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == idCorrGlobali).Select(x => x.VAR_DESC_CORR).FirstOrDefaultAsync();
        }

        private async Task<string> GetCodRubricaCorr(int idCorrGlobali)
        {
            return await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == idCorrGlobali).Select(x => x.VAR_COD_RUBRICA).FirstOrDefaultAsync();
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ModelloTrasmEntity, ModelloTrasmissione>()
                    .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.NOME, src => src.MapFrom(opt => opt.NOME))
                    .ForMember(dest => dest.CHA_TIPO_OGGETTO, src => src.MapFrom(opt => opt.CHA_TIPO_OGGETTO))
                    .ForMember(dest => dest.ID_REGISTRO, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.VAR_NOTE_GENERALI, src => src.MapFrom(opt => opt.VAR_NOTE_GENERALI))
                    .ForMember(dest => dest.ID_PEOPLE, src => src.MapFrom(opt => opt.ID_PEOPLE))
                    .ForMember(dest => dest.SINGLE, src => src.MapFrom(opt => opt.SINGLE))
                    .ForMember(dest => dest.ID_AMM, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.CEDE_DIRITTI, src => src.MapFrom(opt => opt.CHA_CEDE_DIRITTI))
                    .ForMember(dest => dest.ID_PEOPLE_NEW_OWNER, src => src.MapFrom(opt => opt.ID_PEOPLE_NEW_OWNER))
                    .ForMember(dest => dest.ID_GROUP_NEW_OWNER, src => src.MapFrom(opt => opt.ID_GROUP_NEW_OWNER))
                    .ForMember(dest => dest.NO_NOTIFY, src => src.MapFrom(opt => opt.NO_NOTIFY))
                    .ForMember(dest => dest.CODICE, src => src.MapFrom(opt => "MT_" + opt.SYSTEM_ID))
                    .ForMember(dest => dest.MANTIENI_LETTURA, src => src.MapFrom(opt => opt.CHA_MANTIENI_LETTURA))
                    .ForMember(dest => dest.MANTIENI_SCRITTURA, src => src.MapFrom(opt => opt.CHA_MANTIENI_SCRITTURA));

                cfg.CreateMap<ModelloMittDestEntity, MittDest>()
                    .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.ID_MODELLO, src => src.MapFrom(opt => opt.ID_MODELLO))
                    .ForMember(dest => dest.CHA_TIPO_MITT_DEST, src => src.MapFrom(opt => opt.CHA_TIPO_MITT_DEST))
                    .ForMember(dest => dest.ID_RAGIONE, src => src.MapFrom(opt => opt.ID_RAGIONE))
                    .ForMember(dest => dest.CHA_TIPO_TRASM, src => src.MapFrom(opt => opt.CHA_TIPO_TRASM))
                    .ForMember(dest => dest.VAR_NOTE_SING, src => src.MapFrom(opt => opt.VAR_NOTE_SING))
                    .ForMember(dest => dest.CHA_TIPO_URP, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.ID_CORR_GLOBALI, src => src.MapFrom(opt => opt.ID_CORR_GLOBALI))
                    .ForMember(dest => dest.SCADENZA, src => src.MapFrom(opt => opt.SCADENZA != null ? opt.SCADENZA : 0))
                    .ForMember(dest => dest.NASCONDI_VERSIONI_PRECEDENTI, src => src.MapFrom(opt => opt.HIDE_DOC_VERSIONS != null ? opt.HIDE_DOC_VERSIONS.Equals("1") : false));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
