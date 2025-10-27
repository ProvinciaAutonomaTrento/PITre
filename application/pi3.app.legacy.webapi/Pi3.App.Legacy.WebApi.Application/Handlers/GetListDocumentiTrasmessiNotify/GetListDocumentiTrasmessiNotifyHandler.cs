// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Smistamento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetListDocumentiTrasmessiNotifyRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListDocumentiTrasmessiNotify;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListDocumentiTrasmessiNotify
{
    public class GetListDocumentiTrasmessiNotifyHandler : IRequestHandler<GetListDocumentiTrasmessiNotifyRequest, GetListDocumentiTrasmessiNotifyResult>
    {
        #region Public Members

        public GetListDocumentiTrasmessiNotifyHandler(ILogger<GetListDocumentiTrasmessiNotifyHandler> logger,
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

        public async Task<GetListDocumentiTrasmessiNotifyResult> Handle(GetListDocumentiTrasmessiNotifyRequest request, CancellationToken cancellationToken)
        {
            DatiTrasmissioneDocumento[] output = null;

            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente() 
                { 
                    idPeople = request.mittente.IDPeople, 
                    idGruppo = request.mittente.IDGroup 
                };
                var idPeople = request.mittente.IDPeople.AsLong();
                long[] idTrasmSingola = request.notifications.Select(n => n.ID_SPECIALIZED_OBJECT.AsLong()).ToArray();

                var smistaDocInNotifyEntities = await this._dbContext.TrasmissioneEntities
                    .Join(this._dbContext.TrasmSingolaEntities, trasm => trasm.SYSTEM_ID, trasmSing => trasmSing.ID_TRASMISSIONE, (trasm, trasmSingola) => new { trasm, trasmSingola })
                    .Join(this._dbContext.TrasmUtenteEntities, j => j.trasmSingola.SYSTEM_ID, trasmUtente => trasmUtente.ID_TRASM_SINGOLA, (j, trasmUtente) => new { j.trasm, j.trasmSingola, trasmUtente })
                    .Join(this._dbContext.RagioneTrasmissioneEntities, j => j.trasmSingola.ID_RAGIONE, ragione => ragione.SYSTEM_ID, (j, ragione) => new { j.trasm, j.trasmSingola, j.trasmUtente, ragione })
                    .Join(this._dbContext.CorrGlobaliEntities, j => j.trasm.ID_RUOLO_IN_UO, role => role.SYSTEM_ID, (j, role) => new { j.trasm, j.trasmSingola, j.trasmUtente, j.ragione, role })
                    .Join(this._dbContext.PeopleEntities, j => j.trasm.ID_PEOPLE, people => people.SYSTEM_ID, (j, people) => new { j.trasm, j.trasmSingola, j.trasmUtente, j.ragione, j.role, people })
                    .Where(j => j.ragione.CHA_PROC_RES == null && j.trasmUtente.ID_PEOPLE == idPeople && idTrasmSingola.Contains(j.trasmSingola.SYSTEM_ID))
                    .OrderByDescending(j => j.trasm.DTA_INVIO)
                    .Select(j => new SmistaDocInNotifyEntity()
                    {
                        ID_TRASMISSIONE = j.trasm.SYSTEM_ID,
                        ID_TRASMISSIONE_SINGOLA = j.trasmSingola.SYSTEM_ID,
                        ID_TRASMISSIONE_UTENTE = j.trasmUtente.SYSTEM_ID,
                        ID_PROFILE = j.trasm.ID_PROFILE,
                        TIPO_RAGIONE = j.ragione.CHA_TIPO_RAGIONE,
                        NOTE_GENERALI = j.trasm.VAR_NOTE_GENERALI,
                        NOTE_INDIVIDUALI = j.trasmSingola.VAR_NOTE_SING,
                        DESC_RAGIONE_TRASMISSIONE = j.ragione.VAR_DESC_RAGIONE,
                        DESC_ROLE = j.role.VAR_DESC_CORR,
                        DESC_PEOPLE = j.people.FULL_NAME

                    })
                    .ToListAsync();

                List<DatiTrasmissioneDocumento> datiTrasmissioneDocumento = new List<DatiTrasmissioneDocumento>();
                foreach(var datiTrasmissione in smistaDocInNotifyEntities)
                {
                    if(!datiTrasmissioneDocumento.Any(d => d.IDTrasmissione.Equals(datiTrasmissione.ID_TRASMISSIONE.ToString())))
                    {
                        var verificaACLResult = await this._mediator.Send(new VerificaACL("D", datiTrasmissione.ID_PROFILE.ToString(), infoUtente));
                        if(verificaACLResult.output != 0)
                            datiTrasmissioneDocumento.Add(this._mapper.Map<DatiTrasmissioneDocumento>(datiTrasmissione));
                    }
                }

                output = datiTrasmissioneDocumento.ToArray();

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetListDocumentiTrasmessiNotifyResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetListDocumentiTrasmessiNotifyHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SmistaDocInNotifyEntity, DatiTrasmissioneDocumento>()
                   .ForMember(dest => dest.IDDocumento, opt => opt.MapFrom(src => src.ID_PROFILE))
                   .ForMember(dest => dest.IDTrasmissione, opt => opt.MapFrom(src => src.ID_TRASMISSIONE))
                   .ForMember(dest => dest.TrasmissioneConWorkflow, opt => opt.MapFrom(src => src.TIPO_RAGIONE == "W"))
                   .ForMember(dest => dest.IDTrasmissioneSingola, opt => opt.MapFrom(src => src.ID_TRASMISSIONE_SINGOLA))
                   .ForMember(dest => dest.IDTrasmissioneUtente, opt => opt.MapFrom(src => src.ID_TRASMISSIONE_UTENTE))
                   .ForMember(dest => dest.NoteGenerali, opt => opt.MapFrom(src => src.NOTE_GENERALI))
                   .ForMember(dest => dest.NoteIndividualiTrasmSingola, opt => opt.MapFrom(src => src.NOTE_INDIVIDUALI))
                   .ForMember(dest => dest.DescRagioneTrasmissione, opt => opt.MapFrom(src => src.DESC_RAGIONE_TRASMISSIONE))
                   .ForMember(dest => dest.MittenteTrasmissione, opt => opt.MapFrom(src => $"{src.DESC_PEOPLE} ({src.DESC_ROLE})"));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class SmistaDocInNotifyEntity
        {
            public long? ID_TRASMISSIONE { get; set; }
            public long? ID_TRASMISSIONE_SINGOLA { get; set; }
            public long? ID_TRASMISSIONE_UTENTE { get; set; }
            public long? ID_PROFILE { get; set; }
            public string? TIPO_RAGIONE { get; set; }
            public string? NOTE_GENERALI { get; set; }
            public string? NOTE_INDIVIDUALI { get; set; }
            public string? DESC_RAGIONE_TRASMISSIONE { get; set; }
            public string? DESC_ROLE { get; set; }
            public string? DESC_PEOPLE { get; set; }
           

        }

        #endregion
    }
}
