// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.RagioneTrasmissioneAggregate.AggregateModels.RagioneTrasmissioneAggregate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.RagioneTrasmissioneAggregate.AggregateModels.RagioneTrasmissioneAggregate.Repository
{

    public class RagioneTrasmissioneEFRepository : ElementRepository<RagioneTrasmissione>, IRagioneTrasmissioneRepository
    {
        #region Public Members

        public RagioneTrasmissioneEFRepository(ILogger<RagioneTrasmissioneEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;

            InitializeMapper();
        }


        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                //ED tutti true
                //NN tutti false
                //EA solo allegati(AllegatiDocumeti = true)
                //E AllegatiDocumenti false gli altri true

                cfg.CreateMap<RagioneTrasmissioneEntity, RagioneTrasmissione>()
                    .ConstructUsing(src => new RagioneTrasmissione(src.SYSTEM_ID.ToString(), _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true), DateTime.MinValue, new TextValue(src.VAR_DESC_RAGIONE), new TextValue(src.VAR_NOTE)))
                    .AfterMap((src, dest) =>
                    {
                        dest.ChangeOpzioni(new OpzioniRagioneTrasmissione
                        {
                            PrevedeRisposta = src.CHA_TIPO_RISPOSTA == "R",
                            Risposta = src.CHA_RISPOSTA == "1",
                            RagioneDiSistema = src.CHA_RAG_SISTEMA == "1",
                            TipoRagione = GetTipoRagione(src.CHA_TIPO_RAGIONE),
                            TipoDirittoDestinatari = GetTipiDiritti(src.CHA_TIPO_DIRITTI),
                            Visible = src.CHA_VIS == "1",
                            EstendiSuperioriGerarchici = src.CHA_EREDITA == "1",
                            TipoDestinatario = GetTipoDestinatario(src.CHA_TIPO_DEST),
                            TipoCessioneDiritto = GetTipiCessioneDiritti(src.CHA_CEDE_DIRITTI),
                            MantieneDiritti = GetMantieniDiritti(src.CHA_MANTIENI_LETT, src.CHA_MANTIENI_SCRITT),
                            NotificaTramissionePerEmail = new OpzioniNotificaTrasmissionePerEmail
                            {
                                IncludiLinkImmagineDocumento = src.VAR_NOTIFICA_TRASM != null && (src.VAR_NOTIFICA_TRASM.Equals("ED") || src.VAR_NOTIFICA_TRASM.Equals("E")),
                                IncludiLinkSchedaDettaglio = src.VAR_NOTIFICA_TRASM != null && (src.VAR_NOTIFICA_TRASM.Equals("ED") || src.VAR_NOTIFICA_TRASM.Equals("E")),
                                AllegaDocumenti = src.VAR_NOTIFICA_TRASM != null && (src.VAR_NOTIFICA_TRASM.Equals("EA") || src.VAR_NOTIFICA_TRASM.Equals("ED"))
                            },
                            MessaggioNotificaTrasmissione = new OpzioniMessaggioNotificaTrasmissione
                            {
                                TestoPerNotificaDocumenti = src.VAR_TESTO_MSG_NOTIFICA_DOC,
                                TestoPerNotificaFascicoli = src.VAR_TESTO_MSG_NOTIFICA_FASC
                            }
                        });

                        dest.MarkChangesAsCommitted();

                    });
            });

            _mapper = configuration.CreateMapper();
        }

        private TipoRagioneTrasmissioneEnum GetTipoRagione(string cha_tipo_ragione)
        {
            switch (cha_tipo_ragione)
            {
                case "W":
                    return TipoRagioneTrasmissioneEnum.ConWorkflow;
                case "I":
                    return TipoRagioneTrasmissioneEnum.Interoperabilita;
                default:
                    return TipoRagioneTrasmissioneEnum.SenzaWorkflow;
            }
        }

        private TipiDirittiTrasmissioneEnum GetTipiDiritti(string cha_tipo_diritti)
        {
            switch (cha_tipo_diritti)
            {
                case "R":
                    return TipiDirittiTrasmissioneEnum.Lettura;
                case "W":
                    return TipiDirittiTrasmissioneEnum.Scrittura;
                default:
                    return TipiDirittiTrasmissioneEnum.Nessuno;
            }
        }

        private TipiDestinatariTrasmissioneEnum GetTipoDestinatario(string cha_tipo_dest)
        {
            switch (cha_tipo_dest)
            {
                case "I":
                    return TipiDestinatariTrasmissioneEnum.SoloSottoposti;
                case "P":
                    return TipiDestinatariTrasmissioneEnum.PariLivello;
                case "S":
                    return TipiDestinatariTrasmissioneEnum.SoloSuperiori;
                default:
                    return TipiDestinatariTrasmissioneEnum.Tutti;
            }
        }

        private TipiCessioneDirittiEnum GetTipiCessioneDiritti(string cha_cede_diritti)
        {
            switch (cha_cede_diritti)
            {
                case "R":
                    return TipiCessioneDirittiEnum.Si;
                case "W":
                    return TipiCessioneDirittiEnum.SiConSceltaUtente;
                default:
                    return TipiCessioneDirittiEnum.No;
            }
        }

        private TipiDirittiTrasmissioneEnum GetMantieniDiritti(string cha_mantieni_lett, string cha_mantieni_scritt)
        {
            if (cha_mantieni_scritt != null && cha_mantieni_scritt.Equals("1"))
                return TipiDirittiTrasmissioneEnum.Scrittura;

            if (cha_mantieni_lett != null && cha_mantieni_lett.Equals("1"))
                return TipiDirittiTrasmissioneEnum.Lettura;

            return TipiDirittiTrasmissioneEnum.Nessuno;
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();

            var ragioneTrasmissioneEntity = await _dbContext
                        .RagioneTrasmissioneEntities
                        .FirstOrDefaultAsync(p =>
                                p.SYSTEM_ID == idAsLong);

            return ragioneTrasmissioneEntity != null;
        }

        protected override async Task<RagioneTrasmissione> HandleGet(RagioneTrasmissione newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();

            var ragioneEntity = await _dbContext.RagioneTrasmissioneEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == idAsLong && r.ID_AMM == idTenantAsLong);

            return _mapper.Map<RagioneTrasmissione>(ragioneEntity);
        }

        protected override async Task HandleAdd(RagioneTrasmissione aggregate)
        {
            var ragioneEntity = new RagioneTrasmissioneEntity();

            ragioneEntity.VAR_DESC_RAGIONE = aggregate.Name.ToString();
            ragioneEntity.VAR_NOTE = aggregate.Description.ToString();
            ragioneEntity.CHA_RAG_SISTEMA = aggregate.Opzioni.RagioneDiSistema ? "1" : "0";
            ragioneEntity.CHA_TIPO_RISPOSTA = aggregate.Opzioni.PrevedeRisposta ? "R" : "C";
            ragioneEntity.CHA_RISPOSTA = aggregate.Opzioni.Risposta ? "1" : "0";
            ragioneEntity.ID_AMM = aggregate.IdTenant.AsLong();

            switch (aggregate.Opzioni.TipoRagione)
            {
                case TipoRagioneTrasmissioneEnum.SenzaWorkflow:
                    ragioneEntity.CHA_TIPO_RAGIONE = "N";
                    break;
                case TipoRagioneTrasmissioneEnum.ConWorkflow:
                    ragioneEntity.CHA_TIPO_RAGIONE = "W";
                    break;
                case TipoRagioneTrasmissioneEnum.Interoperabilita:
                    ragioneEntity.CHA_TIPO_RAGIONE = "I";
                    break;
            }

            switch (aggregate.Opzioni.TipoDirittoDestinatari)
            {
                case TipiDirittiTrasmissioneEnum.Nessuno:
                    ragioneEntity.CHA_TIPO_DIRITTI = "N";
                    break;
                case TipiDirittiTrasmissioneEnum.Scrittura:
                    ragioneEntity.CHA_TIPO_DIRITTI = "W";
                    break;
                case TipiDirittiTrasmissioneEnum.Lettura:
                    ragioneEntity.CHA_TIPO_DIRITTI = "R";
                    break;
            }

            ragioneEntity.CHA_VIS = aggregate.Opzioni.Visible ? "1" : "0";
            ragioneEntity.CHA_EREDITA = aggregate.Opzioni.EstendiSuperioriGerarchici ? "1" : "0";

            switch (aggregate.Opzioni.TipoDestinatario)
            {
                case TipiDestinatariTrasmissioneEnum.Tutti:
                    ragioneEntity.CHA_TIPO_DEST = "T";
                    break;
                case TipiDestinatariTrasmissioneEnum.SoloSuperiori:
                    ragioneEntity.CHA_TIPO_DEST = "S";
                    break;
                case TipiDestinatariTrasmissioneEnum.SoloSottoposti:
                    ragioneEntity.CHA_TIPO_DEST = "I";
                    break;
                case TipiDestinatariTrasmissioneEnum.PariLivello:
                    ragioneEntity.CHA_TIPO_DEST = "P";
                    break;
            }

            switch (aggregate.Opzioni.TipoCessioneDiritto)
            {
                case TipiCessioneDirittiEnum.Si:
                    ragioneEntity.CHA_CEDE_DIRITTI = "R";
                    break;
                case TipiCessioneDirittiEnum.No:
                    ragioneEntity.CHA_CEDE_DIRITTI = "N";
                    break;
                case TipiCessioneDirittiEnum.SiConSceltaUtente:
                    ragioneEntity.CHA_CEDE_DIRITTI = "W";
                    break;
            }

            switch (aggregate.Opzioni.MantieneDiritti)
            {
                case TipiDirittiTrasmissioneEnum.Nessuno:
                    ragioneEntity.CHA_MANTIENI_LETT = "0";
                    ragioneEntity.CHA_MANTIENI_SCRITT = "0";
                    break;
                case TipiDirittiTrasmissioneEnum.Lettura:
                    ragioneEntity.CHA_MANTIENI_LETT = "1";
                    ragioneEntity.CHA_MANTIENI_SCRITT = "0";
                    break;
                case TipiDirittiTrasmissioneEnum.Scrittura:
                    ragioneEntity.CHA_MANTIENI_LETT = "1";
                    ragioneEntity.CHA_MANTIENI_SCRITT = "1";
                    break;
            }

            if (aggregate.Opzioni.NotificaTramissionePerEmail != null)
            {
                if (aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "ED";

                if (aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "E";

                if (!aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "EA";

                if (!aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "NN";
            }

            if (aggregate.Opzioni.MessaggioNotificaTrasmissione != null)
            {
                ragioneEntity.VAR_TESTO_MSG_NOTIFICA_DOC = aggregate.Opzioni.MessaggioNotificaTrasmissione.TestoPerNotificaDocumenti;
                ragioneEntity.VAR_TESTO_MSG_NOTIFICA_FASC = aggregate.Opzioni.MessaggioNotificaTrasmissione.TestoPerNotificaFascicoli;
            }

            await _dbContext.RagioneTrasmissioneEntities.AddAsync(ragioneEntity);

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleDelete(RagioneTrasmissione aggregate)
        {
            var idTenantAsLong = aggregate.IdTenant.AsLong();
            var idAsLong = aggregate.Id.AsLong();

            var ragioneEntity = await _dbContext.RagioneTrasmissioneEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == idAsLong && r.ID_AMM == idTenantAsLong);

            if (ragioneEntity == null)
                throw new RagioneTrasmissioneNotFoundPi3Exception(aggregate.Id);

            _dbContext.RagioneTrasmissioneEntities.Remove(ragioneEntity);

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleUpdate(RagioneTrasmissione aggregate)
        {
            var idTenantAsLong = aggregate.IdTenant.AsLong();
            var idAsLong = aggregate.Id.AsLong();

            var ragioneEntity = await _dbContext.RagioneTrasmissioneEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == idAsLong && r.ID_AMM == idTenantAsLong);

            if (ragioneEntity == null)
                throw new RagioneTrasmissioneNotFoundPi3Exception(aggregate.Id);

            ragioneEntity.CHA_RAG_SISTEMA = aggregate.Opzioni.RagioneDiSistema ? "1" : "0";
            ragioneEntity.CHA_RAG_SISTEMA = aggregate.Opzioni.RagioneDiSistema ? "1" : "0";
            ragioneEntity.CHA_TIPO_RISPOSTA = aggregate.Opzioni.PrevedeRisposta ? "R" : "C";

            switch (aggregate.Opzioni.TipoRagione)
            {
                case TipoRagioneTrasmissioneEnum.SenzaWorkflow:
                    ragioneEntity.CHA_TIPO_RAGIONE = "N";
                    break;
                case TipoRagioneTrasmissioneEnum.ConWorkflow:
                    ragioneEntity.CHA_TIPO_RAGIONE = "W";
                    break;
                case TipoRagioneTrasmissioneEnum.Interoperabilita:
                    ragioneEntity.CHA_TIPO_RAGIONE = "I";
                    break;
            }

            switch (aggregate.Opzioni.TipoDirittoDestinatari)
            {
                case TipiDirittiTrasmissioneEnum.Nessuno:
                    ragioneEntity.CHA_TIPO_DIRITTI = "N";
                    break;
                case TipiDirittiTrasmissioneEnum.Scrittura:
                    ragioneEntity.CHA_TIPO_DIRITTI = "W";
                    break;
                case TipiDirittiTrasmissioneEnum.Lettura:
                    ragioneEntity.CHA_TIPO_DIRITTI = "R";
                    break;
            }

            ragioneEntity.CHA_VIS = aggregate.Opzioni.Visible ? "1" : "0";
            ragioneEntity.CHA_EREDITA = aggregate.Opzioni.EstendiSuperioriGerarchici ? "1" : "0";

            switch (aggregate.Opzioni.TipoDestinatario)
            {
                case TipiDestinatariTrasmissioneEnum.Tutti:
                    ragioneEntity.CHA_TIPO_DEST = "T";
                    break;
                case TipiDestinatariTrasmissioneEnum.SoloSuperiori:
                    ragioneEntity.CHA_TIPO_DEST = "S";
                    break;
                case TipiDestinatariTrasmissioneEnum.SoloSottoposti:
                    ragioneEntity.CHA_TIPO_DEST = "I";
                    break;
                case TipiDestinatariTrasmissioneEnum.PariLivello:
                    ragioneEntity.CHA_TIPO_DEST = "P";
                    break;
            }

            switch (aggregate.Opzioni.TipoCessioneDiritto)
            {
                case TipiCessioneDirittiEnum.Si:
                    ragioneEntity.CHA_CEDE_DIRITTI = "R";
                    break;
                case TipiCessioneDirittiEnum.No:
                    ragioneEntity.CHA_CEDE_DIRITTI = "N";
                    break;
                case TipiCessioneDirittiEnum.SiConSceltaUtente:
                    ragioneEntity.CHA_CEDE_DIRITTI = "W";
                    break;
            }

            switch (aggregate.Opzioni.MantieneDiritti)
            {
                case TipiDirittiTrasmissioneEnum.Nessuno:
                    ragioneEntity.CHA_MANTIENI_LETT = "0";
                    ragioneEntity.CHA_MANTIENI_SCRITT = "0";
                    break;
                case TipiDirittiTrasmissioneEnum.Lettura:
                    ragioneEntity.CHA_MANTIENI_LETT = "1";
                    ragioneEntity.CHA_MANTIENI_SCRITT = "0";
                    break;
                case TipiDirittiTrasmissioneEnum.Scrittura:
                    ragioneEntity.CHA_MANTIENI_LETT = "1";
                    ragioneEntity.CHA_MANTIENI_SCRITT = "1";
                    break;
            }

            if (aggregate.Opzioni.NotificaTramissionePerEmail != null)
            {
                if (aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "ED";

                if (aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "E";

                if (!aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "EA";

                if (!aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkSchedaDettaglio &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.IncludiLinkImmagineDocumento &&
                    !aggregate.Opzioni.NotificaTramissionePerEmail.AllegaDocumenti)
                    ragioneEntity.VAR_NOTIFICA_TRASM = "NN";
            }

            if (aggregate.Opzioni.MessaggioNotificaTrasmissione != null)
            {
                ragioneEntity.VAR_TESTO_MSG_NOTIFICA_DOC = aggregate.Opzioni.MessaggioNotificaTrasmissione.TestoPerNotificaDocumenti;
                ragioneEntity.VAR_TESTO_MSG_NOTIFICA_FASC = aggregate.Opzioni.MessaggioNotificaTrasmissione.TestoPerNotificaFascicoli;
            }

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        #endregion
    }

}
