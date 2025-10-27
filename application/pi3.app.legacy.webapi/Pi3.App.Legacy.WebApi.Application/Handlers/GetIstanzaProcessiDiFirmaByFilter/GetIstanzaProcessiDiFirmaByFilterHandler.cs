// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
using MediatR;
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
using GetIstanzaProcessiDiFirmaByFilterRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetIstanzaProcessiDiFirmaByFilter;
using getFirstDayOfWeekRequest = Pi3.App.Legacy.WebApi.Application.Requests.getFirstDayOfWeek;
using getLastDayOfWeekRequest = Pi3.App.Legacy.WebApi.Application.Requests.getLastDayOfWeek;
using getFirstDayOfMonthRequest = Pi3.App.Legacy.WebApi.Application.Requests.getFirstDayOfMonth;
using getLastDayOfMonthRequest = Pi3.App.Legacy.WebApi.Application.Requests.getLastDayOfMonth;
using GetYesterdayRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetYesterday;
using GetLastSevenDayRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetLastSevenDay;
using GetLastThirtyOneDayRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetLastThirtyOneDay;
using DocsPaVO.DiagrammaStato;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Services.Configuration;
using AutoMapper;
using DocsPaVO.utente;
using DocsPaVO.documento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetIstanzaProcessiDiFirmaByFilter
{
    public class GetIstanzaProcessiDiFirmaByFilterHandler : IRequestHandler<GetIstanzaProcessiDiFirmaByFilterRequest, GetIstanzaProcessiDiFirmaByFilterResult>
    {
        #region Public Members

        public GetIstanzaProcessiDiFirmaByFilterHandler(ILogger<GetIstanzaProcessiDiFirmaByFilterHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;

            this.InitializeMapper();
        }

        public async Task<GetIstanzaProcessiDiFirmaByFilterResult> Handle(GetIstanzaProcessiDiFirmaByFilterRequest request, CancellationToken cancellationToken)
        {
            IstanzaProcessoDiFirma[] output = null;
            var numTotPage = 0;
            var nRec = 0;
            DataSet istanzeProcessi = null;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var joinTableIstanzaPassi = request.filtro.Any(f => (f.Argomento == "RUOLO_COINVOLTO" || f.Argomento == "UTENTE_COINVOLTO" || f.Argomento == "TIPO_PASSO_AUTOMATICO") 
                            && !string.IsNullOrEmpty(f.Valore));

                var joinTableVisibilitaProcessi = request.filtro.Any(f => (f.Argomento == "PROPONENTE_AVVIATI_DA_ME" || f.Argomento == "PROPONENTE_AVVIATI_DAL_RUOLO" || f.Argomento == "MONITORAGGIO")
                            && Convert.ToBoolean(f.Valore)) || !request.filtro.Any(f => f.Argomento == "FROM_UTILIZZO" && Convert.ToBoolean(f.Valore)); 
             
                var queryable = this._dbContext.IstanzaProcessoFirmaEntities
                    .Join(this._dbContext.ProfileEntities, istanza => istanza.ID_DOCUMENTO, profile => profile.DOCNUMBER, (istanza, profile) => new { istanza, profile })
                    .Join(this._dbContext.CorrGlobaliEntities, j => j.istanza.ID_RUOLO_PROPONENTE, ruoloProponente => ruoloProponente.ID_GRUPPO, (j, ruoloProponente) => new { j.istanza, j.profile, ruoloProponente })
                    .Join(this._dbContext.CorrGlobaliEntities, j => j.istanza.ID_UTENTE_PROPONENTE, utenteProponente => utenteProponente.ID_PEOPLE, (j, utenteProponente) => 
                    new IstanzaProcessiFirmaQueryEntity()
                    { 
                        IstanzaProcessoFirma = j.istanza, 
                        Profile = j.profile, 
                        RuoloProponente = new RuoloProponenteEntity()
                        {
                             ID_GROUP = j.ruoloProponente.ID_GRUPPO,
                             GROUP_DESCRIPTION = j.ruoloProponente.VAR_DESC_CORR,
                             GROUP_ID = j.ruoloProponente.VAR_COD_RUBRICA,
                             ID_CORR_GLOBALI_RUOLO = j.ruoloProponente.SYSTEM_ID
                        }, 
                        UtenteProponente = new UtenteProponenteEntity()
                        {
                             ID_CORR_GLOBALI_UTENTE = utenteProponente.SYSTEM_ID,
                             ID_USER = utenteProponente.ID_PEOPLE,
                             USER_DESCRIPTION = utenteProponente.VAR_DESC_CORR,
                             USER_ID = utenteProponente.VAR_COD_RUBRICA
                        },
                        IstanzaPassoFirma = null,
                        ProcessoFirmaVisibilita = null
                    });
                 
                if(joinTableIstanzaPassi)
                {
                    queryable = queryable.Join(this._dbContext.IstanzaPassoFirmaEntities, j => j.IstanzaProcessoFirma.ID_ISTANZA, passi => passi.ID_ISTANZA_PROCESSO, (j, passi) =>
                    new IstanzaProcessiFirmaQueryEntity()
                    {
                        IstanzaProcessoFirma = j.IstanzaProcessoFirma,
                        Profile = j.Profile,
                        RuoloProponente = j.RuoloProponente,
                        UtenteProponente = j.UtenteProponente,
                        IstanzaPassoFirma = passi,
                        ProcessoFirmaVisibilita = j.ProcessoFirmaVisibilita
                    });
                }

                if (joinTableVisibilitaProcessi)
                {
                    queryable = queryable.Join(this._dbContext.ProcessoFirmaVisibilitaEntities, j => j.IstanzaProcessoFirma.ID_PROCESSO, visibilita => visibilita.ID_PROCESSO, (j, visibilita) =>
                    new IstanzaProcessiFirmaQueryEntity()
                    {
                        IstanzaProcessoFirma = j.IstanzaProcessoFirma,
                        Profile = j.Profile,
                        RuoloProponente = j.RuoloProponente,
                        UtenteProponente = j.UtenteProponente,
                        IstanzaPassoFirma = j.IstanzaPassoFirma,
                        ProcessoFirmaVisibilita = visibilita
                    })
                    .Where(j => j.ProcessoFirmaVisibilita.ID_GROUPS == idGroup && j.IstanzaProcessoFirma.ATTIVATO_IL.Date >= (j.ProcessoFirmaVisibilita.DTA_INIZIO ?? DateTime.MinValue).Date 
                        && j.IstanzaProcessoFirma.ATTIVATO_IL.Date <= (j.ProcessoFirmaVisibilita.DTA_FINE ?? DateTime.MaxValue).Date);
                }

                queryable = queryable.Where(j => (j.Profile.CHA_IN_CESTINO ?? "0") == "0");


                #region COSTRUZIONE FILTRI DI RICERCA

                var today = (await this._dbContext.GetSystemDateTime()).Date;
                var firstDayOfWeek = (await this._mediator.Send(new getFirstDayOfWeekRequest())).output.AsDateTime().Date;
                var lastDayOfWeek = (await this._mediator.Send(new getLastDayOfWeekRequest())).output.AsDateTime().Date;
                var firstDayOfMonth = (await this._mediator.Send(new getFirstDayOfMonthRequest())).output.AsDateTime().Date;
                var lastDayOfMonth = (await this._mediator.Send(new getLastDayOfMonthRequest())).output.AsDateTime().Date;
                var yesterday = (await this._mediator.Send(new GetYesterdayRequest())).output.AsDateTime().Date;
                var lastSevenDay = (await this._mediator.Send(new GetLastSevenDayRequest())).output.AsDateTime().Date;
                var lastThirtyOneDay = (await this._mediator.Send(new GetLastThirtyOneDayRequest())).output.AsDateTime().Date;
                List<string> stati = new List<string>();
                ExpressionStarter<IstanzaProcessiFirmaQueryEntity> predicateVisibilita = null;

                foreach (FiltroIstanzeProcessoFirma f in request.filtro)
                {
                    switch (f.Argomento)
                    {
                        case "ID_PROCESSO":
                            if (!string.IsNullOrEmpty(f.Valore))
                            {
                                var idProcesso = f.Valore.AsLong();
                                queryable = queryable.Where(j => j.IstanzaProcessoFirma.ID_PROCESSO == idProcesso);
                            }
                            break;
                        case "NOME":
                            var nome = f.Valore.ToUpper();
                            //queryable = queryable.Where(j => j.IstanzaProcessoFirma.DESCRIZIONE.ToUpper().Contains(nome));
                            queryable = queryable.Where(j => EF.Functions.Like(j.IstanzaProcessoFirma.DESCRIZIONE.ToUpper(), $"%{nome}%"));
                            break;
                        case "NOTE_AVVIO":
                            var note = f.Valore.Trim().ToUpper();
                            //queryable = queryable.Where(j => j.IstanzaProcessoFirma.NOTE.ToUpper().Contains(note));
                            queryable = queryable.Where(j => EF.Functions.Like(j.IstanzaProcessoFirma.NOTE.ToUpper(), $"%{note}%"));
                            break;
                        case "NOTE_RESPINGIMENTO":
                            var noteRespingimento = f.Valore.ToUpper();
                            //queryable = queryable.Where(j => j.IstanzaProcessoFirma.MOTIVO_RESPINGIMENTO.ToUpper().Contains(noteRespingimento));
                            queryable = queryable.Where(j => EF.Functions.Like(j.IstanzaProcessoFirma.MOTIVO_RESPINGIMENTO.ToUpper(), $"%{noteRespingimento}%"));
                            break;
                        case "STATO_IN_ESECUZIONE":
                            if (Convert.ToBoolean(f.Valore))
                                stati.Add(DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC.ToString());
                            break;
                        case "STATO_IN_ERRORE":
                            if (Convert.ToBoolean(f.Valore))
                            {
                                stati.Add(DocsPaVO.LibroFirma.TipoStatoProcesso.IN_ERROR.ToString());
                                stati.Add(DocsPaVO.LibroFirma.TipoStatoProcesso.REPLAY.ToString());
                            }
                            break;
                        case "STATO_INTERROTTO":
                            if (Convert.ToBoolean(f.Valore))
                                stati.Add(DocsPaVO.LibroFirma.TipoStatoProcesso.STOPPED.ToString());
                            break;
                        case "STATO_CONCLUSO":
                            if (Convert.ToBoolean(f.Valore))
                                stati.Add(DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED.ToString());
                            break;
                        case "TRONCATO": //LA CONDIZIONE TRONCATO VA IN OR CON LA CONDIZIONE STATO
                            if (Convert.ToBoolean(f.Valore))
                                queryable = queryable.Where(j => this._dbContext.IstanzaPassoFirmaEntities.Any(i => i.ID_ISTANZA_PROCESSO == j.IstanzaProcessoFirma.ID_ISTANZA && i.STATO_PASSO == "CUT"));
                            break;
                        case "DOCNUMBER":
                            var docnumber = f.Valore.AsLong();
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ID_DOCUMENTO == docnumber);
                            break;
                        case "DOCNUMBER_DAL":
                            var docnumberDal = f.Valore.AsLong();
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ID_DOCUMENTO >= docnumberDal);
                            break;
                        case "DOCNUMBER_AL":
                            var docnumberAl = f.Valore.AsLong();
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ID_DOCUMENTO <= docnumberAl);
                            break;
                        case "OGGETTO":
                            var oggetto = f.Valore.ToUpper();
                            //queryable = queryable.Where(j => oggetto.Contains(j.Profile.VAR_PROF_OGGETTO.ToUpper()));
                            queryable = queryable.Where(j => EF.Functions.Like(j.Profile.VAR_PROF_OGGETTO.ToUpper(), $"%{oggetto}%"));
                            break;
                        case "DATA_AVVIO_IL":
                            var attivatoIl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date == attivatoIl);
                            break;
                        case "DATA_AVVIO_SUCCESSIVA_AL":
                            var attivatoIlAl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date >= attivatoIlAl);
                            break;
                        case "DATA_AVVIO_PRECEDENTE_IL":
                            var attivatoIlIl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date <= attivatoIlIl);
                            break;
                        case "DATA_AVVIO_SC":
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date >= firstDayOfWeek && j.IstanzaProcessoFirma.ATTIVATO_IL.Date <= lastDayOfWeek);
                            break;
                        case "DATA_AVVIO_MC":
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date >= firstDayOfMonth && j.IstanzaProcessoFirma.ATTIVATO_IL.Date <= lastDayOfMonth);
                            break;
                        case "DATA_AVVIO_TODAY":
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date == today);
                            break;
                        case "DATA_AVVIO_YESTERDAY":
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date == yesterday);
                            break;
                        case "DATA_AVVIO_LAST_SEVEN_DAYS":
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date >= lastSevenDay);
                            break;
                        case "DATA_AVVIO_LAST_THIRTY_ONE_DAYS":
                            queryable = queryable.Where(j => j.IstanzaProcessoFirma.ATTIVATO_IL.Date >= lastThirtyOneDay);
                            break;
                        case "DATA_CONCLUSIONE_IL":
                            var conclusoIl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date == conclusoIl);
                            break;
                        case "DATA_CONCLUSIONE_SUCCESSIVA_AL":
                            var conclusoIlAl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= conclusoIlAl);
                            break;
                        case "DATA_CONCLUSIONE_PRECEDENTE_IL":
                            var conclusoIlIl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= conclusoIlIl);
                            break;
                        case "DATA_CONCLUSIONE_SC":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= firstDayOfWeek && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfWeek);
                            break;
                        case "DATA_CONCLUSIONE_MC":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= firstDayOfMonth && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfMonth);
                            break;
                        case "DATA_CONCLUSIONE_TODAY":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date == today);
                            break;
                        case "DATA_CONCLUSIONE_YESTERDAY":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date == yesterday);
                            break;
                        case "DATA_CONCLUSIONE_LAST_SEVEN_DAYS":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= lastSevenDay);
                            break;
                        case "DATA_CONCLUSIONE_LAST_THIRTY_ONE_DAYS":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= lastThirtyOneDay);
                            break;
                        case "DATA_INTERRUZIONE_IL":
                            var interrottoIl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date == interrottoIl && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_SUCCESSIVA_AL":
                            var interrottoAl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= interrottoAl);
                            break;
                        case "DATA_INTERRUZIONE_PRECEDENTE_IL":
                            var interrottoPrecIl = f.Valore.AsDateTime().Date;
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= interrottoPrecIl && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_SC":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= firstDayOfWeek && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfWeek && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_MC":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= firstDayOfMonth && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfMonth && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_TODAY":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date == today && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfMonth && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_YESTERDAY":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date == yesterday && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfMonth && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_LAST_SEVEN_DAYS":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= lastSevenDay && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfMonth && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "DATA_INTERRUZIONE_LAST_THIRTY_ONE_DAYS":
                            queryable = queryable.Where(j => (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date >= lastThirtyOneDay && (j.IstanzaProcessoFirma.CONCLUSO_IL ?? DateTime.MinValue).Date <= lastDayOfMonth && j.IstanzaProcessoFirma.STATO == DocsPaVO.LibroFirma.StatoProcesso.STOPPED);
                            break;
                        case "TIPO_PASSO_AUTOMATICO":
                            if (!string.IsNullOrEmpty(f.Valore))
                            {
                                queryable = queryable.Where(j => j.IstanzaPassoFirma.CHA_AUTOMATICO == "1" && j.IstanzaPassoFirma.TIPO_FIRMA == f.Valore);
                            }
                            break;
                        case "RUOLO_COINVOLTO":
                            if (!string.IsNullOrEmpty(f.Valore))
                            {
                                var idCorrGlobaliRuolo = f.Valore.AsLong();
                                var idGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliRuolo).Select(c => c.ID_GRUPPO).FirstAsync();
                                queryable = queryable.Where(j => j.IstanzaPassoFirma.ID_RUOLO_COINVOLTO == idGruppo);
                            }
                            break;
                        case "UTENTE_COINVOLTO":
                            if (!string.IsNullOrEmpty(f.Valore))
                            {
                                var idCorrGlobaliUtente = f.Valore.AsLong();
                                var idPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliUtente).Select(c => c.ID_PEOPLE).FirstAsync();
                                queryable = queryable.Where(j => j.IstanzaPassoFirma.ID_UTENTE_COINVOLTO == idPeople);
                            }
                            break;
                        case "PROPONENTE_AVVIATI_DA_ME":
                            if (Convert.ToBoolean(f.Valore))
                            {
                                var idRuoloProponente = (from f1 in request.filtro where f1.Argomento == "ID_RUOLO_PROPONENTE" select f1.Valore.AsLong()).First();
                                var idUtenteProponente = (from f1 in request.filtro where f1.Argomento == "ID_UTENTE_PROPONENTE" select f1.Valore.AsLong()).First();

                                if (predicateVisibilita == null)
                                    predicateVisibilita = PredicateBuilder.New<IstanzaProcessiFirmaQueryEntity>();

                                predicateVisibilita = predicateVisibilita.Or(j => j.IstanzaProcessoFirma.ID_RUOLO_PROPONENTE == idRuoloProponente && j.IstanzaProcessoFirma.ID_UTENTE_PROPONENTE == idUtenteProponente);
                            }
                            break;
                        case "PROPONENTE_AVVIATI_DAL_RUOLO":
                            if (Convert.ToBoolean(f.Valore))
                            {
                                var idRuoloProponente = (from f1 in request.filtro where f1.Argomento == "ID_RUOLO_PROPONENTE" select f1.Valore.AsLong()).First();

                                if (predicateVisibilita == null)
                                    predicateVisibilita = PredicateBuilder.New<IstanzaProcessiFirmaQueryEntity>();

                                predicateVisibilita = predicateVisibilita.Or(j => j.IstanzaProcessoFirma.ID_RUOLO_PROPONENTE == idRuoloProponente);
                            }
                            break;
                        case "MONITORAGGIO":
                            if (Convert.ToBoolean(f.Valore))
                            {
                                var idRuoloProponente = (from f1 in request.filtro where f1.Argomento == "ID_RUOLO_PROPONENTE" select f1.Valore.AsLong()).First();

                                if (predicateVisibilita == null)
                                    predicateVisibilita = PredicateBuilder.New<IstanzaProcessiFirmaQueryEntity>();

                                string[] tipoVisibilita = new string[] { "M", "T" };
                                predicateVisibilita = predicateVisibilita.Or(j => tipoVisibilita.Contains(j.ProcessoFirmaVisibilita.CHA_TIPO_VISIBILITA) && j.IstanzaProcessoFirma.ID_RUOLO_PROPONENTE != idRuoloProponente);
                            }
                            break;
                    }
                }
                if (stati.Any())
                    queryable = queryable.Where(j => stati.Contains(j.IstanzaProcessoFirma.STATO));

                if (predicateVisibilita != null)
                    queryable = queryable.Where(predicateVisibilita);
                #endregion

                long?[] personorgroup = new long?[] { idUser, idGroup };
                var result = await queryable
                    .OrderByDescending(j => j.IstanzaProcessoFirma.ATTIVATO_IL)
                    .ThenByDescending(j => j.IstanzaProcessoFirma.ID_ISTANZA)
                    .ThenBy(j => j.IstanzaProcessoFirma.DESCRIZIONE)
                    .Select(j => new
                    {
                        j.IstanzaProcessoFirma.ID_ISTANZA,
                        j.IstanzaProcessoFirma.ID_DOCUMENTO,
                        SECURITY = this._dbContext.SecurityEntities.Any(s => s.THING == (j.Profile.ID_DOCUMENTO_PRINCIPALE == null ? j.Profile.DOCNUMBER : j.Profile.ID_DOCUMENTO_PRINCIPALE)
                            && personorgroup.Contains(s.PERSONORGROUP) && s.ACCESSRIGHTS > 0) ? "1" : "0"
                    })
                    .AsNoTracking()
                    .ToListAsync();
                istanzeProcessi = result.DistinctBy(j => j.ID_ISTANZA).AsDataSet();
                nRec = istanzeProcessi.Tables[0].Rows.Count;

                if (nRec > 0)
                {
                    numTotPage = (nRec / request.pageSize);
                    int startRow = ((request.numPage * request.pageSize) - request.pageSize) + 1;
                    int endRow = (startRow - 1) + request.pageSize;

                    var istanzaProcessoFirmaEntity = await queryable
                        .OrderByDescending(j => j.IstanzaProcessoFirma.ATTIVATO_IL)
                        .ThenByDescending(j => j.IstanzaProcessoFirma.ID_ISTANZA)
                        .ThenBy(j => j.IstanzaProcessoFirma.DESCRIZIONE)
                        .Skip(startRow -1)
                        .Take(request.pageSize)
                        .Select(j => new IstanzaProcessiFirmaResultEntity()
                        {
                            IstanzaProcessoFirma = new IstanzaProcessoFirmaEntity()
                            {
                                ID_ISTANZA = j.IstanzaProcessoFirma.ID_ISTANZA,
                                ID_PROCESSO = j.IstanzaProcessoFirma.ID_PROCESSO,
                                ID_DOCUMENTO = j.IstanzaProcessoFirma.ID_DOCUMENTO,
                                DOC_ALL = j.IstanzaProcessoFirma.DOC_ALL,
                                DESCRIZIONE = j.IstanzaProcessoFirma.DESCRIZIONE,
                                ATTIVATO_IL = j.IstanzaProcessoFirma.ATTIVATO_IL,
                                CONCLUSO_IL = j.IstanzaProcessoFirma.CONCLUSO_IL,
                                NOTE = j.IstanzaProcessoFirma.NOTE,
                                MOTIVO_RESPINGIMENTO = j.IstanzaProcessoFirma.MOTIVO_RESPINGIMENTO,
                                STATO = j.IstanzaProcessoFirma.STATO,
                            },
                            RuoloProponente = j.RuoloProponente,
                            UtenteProponente = j.UtenteProponente,
                            Documento = new DocumentoEntity()
                            {
                                NUM_PROTO = j.Profile.NUM_PROTO,
                                DTA_ANNULLA = j.Profile.DTA_ANNULLA,
                                DTA_PROTO = j.Profile.DTA_PROTO,
                                CREATION_DATE = j.Profile.CREATION_DATE,
                                VAR_PROF_OGGETTO = j.Profile.VAR_PROF_OGGETTO + (this._dbContext.SecurityEntities.Any(s => s.THING == (j.Profile.ID_DOCUMENTO_PRINCIPALE == null ? j.Profile.DOCNUMBER : j.Profile.ID_DOCUMENTO_PRINCIPALE)
                                && personorgroup.Contains(s.PERSONORGROUP) && s.ACCESSRIGHTS > 0) ? "#1" : "#0"),
                            }
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    istanzaProcessoFirmaEntity.ForEach(i =>
                    {
                        var docnumber = i.IstanzaProcessoFirma.ID_DOCUMENTO.ToString();
                        i.Documento.VAR_SEGNATURA_REPERTORIO = this._dbContext.AssociazioneTemplatesEntities
                            .Join(this._dbContext.OggettiCustomEntities, ass => ass.ID_OGGETTO, ogg => ogg.SYSTEM_ID, (ass, ogg) => new { ass, ogg })
                            .Where(j => j.ass.DOC_NUMBER == docnumber && j.ogg.REPERTORIO == 1)
                            .Select(j => j.ass.VAR_SEGNATURA)
                            .FirstOrDefault();
                         
                        if(i.IstanzaProcessoFirma.STATO == "IN_EXEC" || i.IstanzaProcessoFirma.STATO == "IN_ERROR" || i.IstanzaProcessoFirma.STATO == "REPLAY")
                        {
                            var istanzaPassoAttesa = this._dbContext.IstanzaPassoFirmaEntities
                                .Join(this._dbContext.AnagraficaEventiEntities, passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) =>
                                new IstanzaPassoFirmaEventoEntity()
                                {
                                     IstanzaPassoFirma = passo,
                                     Evento = new AnagraficaEventiEntity
                                     {
                                         ID_EVENTO = evento.ID_EVENTO,
                                         VAR_COD_AZIONE = evento.VAR_COD_AZIONE,
                                         CHA_TIPO_EVENTO = evento.CHA_TIPO_EVENTO,
                                         GRUPPO = evento.GRUPPO
                                     }
                                })
                                .Where(p => p.IstanzaPassoFirma.ID_ISTANZA_PROCESSO == i.IstanzaProcessoFirma.ID_ISTANZA && p.IstanzaPassoFirma.STATO_PASSO == "LOOK")
                                .ToList();
                            i.istanzePassoDiFirma = istanzaPassoAttesa;
                        }

                        i.IS_TRONCATO = this._dbContext.IstanzaPassoFirmaEntities.Any(p => p.ID_ISTANZA_PROCESSO == i.IstanzaProcessoFirma.ID_ISTANZA && p.STATO_PASSO == "CUT");
                        if(i.IstanzaProcessoFirma.STATO == "CLOSED" && i.IS_TRONCATO)
                        {
                            i.IstanzaProcessoFirma.STATO = "CLOSED_WITH_CUT";
                        }
                    });

                    output = this._mapper.Map<IstanzaProcessoDiFirma[]>(istanzaProcessoFirmaEntity);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetIstanzaProcessiDiFirmaByFilterResult(output, numTotPage, nRec, istanzeProcessi);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIstanzaProcessiDiFirmaByFilterHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IstanzaProcessiFirmaResultEntity, IstanzaProcessoDiFirma>()
                     .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_ISTANZA))
                     .ForMember(dest => dest.oggetto, opt => opt.MapFrom(src => src.Documento.VAR_PROF_OGGETTO))
                     .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_PROCESSO))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.DESCRIZIONE))
                     .ForMember(dest => dest.dataAttivazione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ATTIVATO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.dataChiusura, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CONCLUSO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.statoProcesso, opt => opt.MapFrom(src => (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), src.IstanzaProcessoFirma.STATO)))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_DOCUMENTO))
                     .ForMember(dest => dest.MotivoRespingimento, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.MOTIVO_RESPINGIMENTO ?? string.Empty))
                     .ForMember(dest => dest.NoteDiAvvio, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.NOTE ?? string.Empty))
                     .ForMember(dest => dest.docAll, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.DOC_ALL ?? string.Empty))
                     .ForMember(dest => dest.IsTroncato, opt => opt.MapFrom(src => src.IS_TRONCATO))
                     .ForMember(dest => dest.SegnaturaRepertorio, opt => opt.MapFrom(src => src.Documento.VAR_SEGNATURA_REPERTORIO ?? string.Empty))
                     .ForMember(dest => dest.NumeroProtocollo, opt => opt.MapFrom(src => src.Documento.NUM_PROTO.HasValue ? src.Documento.NUM_PROTO.ToString() : string.Empty))
                     .ForMember(dest => dest.DataProtocollazione, opt => opt.MapFrom(src => src.Documento.DTA_PROTO.HasValue ? src.Documento.DTA_PROTO.AsDateFormat() : string.Empty))
                     .ForMember(dest => dest.DataCreazione, opt => opt.MapFrom(src => src.Documento.CREATION_DATE.HasValue ? src.Documento.CREATION_DATE.AsDateFormat() : string.Empty))
                     .ForMember(dest => dest.DataAnnullamentoProtocollazione, opt => opt.MapFrom(src => src.Documento.DTA_ANNULLA.HasValue ? src.Documento.DTA_ANNULLA.AsDateFormat() : string.Empty));

                cfg.CreateMap<RuoloProponenteEntity, Ruolo>()
                   .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GROUP))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.GROUP_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.GROUP_DESCRIPTION))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI_RUOLO));

                cfg.CreateMap<UtenteProponenteEntity, Utente>()
                   .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.ID_USER))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.USER_DESCRIPTION))
                   .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI_UTENTE));

                cfg.CreateMap<IstanzaPassoFirmaEventoEntity, IstanzaPassoDiFirma>()
                    .ForMember(dest => dest.CodiceTipoEvento, opt => opt.MapFrom(src => src.IstanzaPassoFirma.TIPO_FIRMA))
                    .ForMember(dest => dest.dataEsecuzione, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ESEGUITO_IL.AsDateTimeFormat()))
                    .ForMember(dest => dest.dataScadenza, opt => opt.MapFrom(src => src.IstanzaPassoFirma.SCADENZA.AsDateTimeFormat()))
                    .ForMember(dest => dest.statoPasso, opt => opt.MapFrom(src => (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), src.IstanzaPassoFirma.STATO_PASSO)))
                    .ForMember(dest => dest.idIstanzaPasso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_ISTANZA_PASSO))
                    .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_ISTANZA_PROCESSO))
                    .ForMember(dest => dest.idNotificaEffettuata, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_NOTIFICA_EFFETTUATA.HasValue ? src.IstanzaPassoFirma.ID_NOTIFICA_EFFETTUATA.ToString() : string.Empty ))
                    .ForMember(dest => dest.idPasso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_PASSO))
                    .ForMember(dest => dest.motivoRespingimento, opt => opt.MapFrom(src => src.IstanzaPassoFirma.MOTIVO_RESPINGIMENTO ?? string.Empty))
                    .ForMember(dest => dest.numeroSequenza, opt => opt.MapFrom(src => src.IstanzaPassoFirma.NUMERO_SEQUENZA))
                    .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.IstanzaPassoFirma.NOTE ?? string.Empty))
                    .ForMember(dest => dest.TipoFirma, opt => opt.MapFrom(src => src.IstanzaPassoFirma.TIPO_FIRMA))
                    .ForMember(dest => dest.IdAOO, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_AOO))
                    .ForMember(dest => dest.IdRF, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_RF))
                    .ForMember(dest => dest.IdMailRegistro, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_MAIL_REGISTRO))
                    .ForMember(dest => dest.IdTipologia, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_TIPOLOGIA))
                    .ForMember(dest => dest.IdStatoDiagramma, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_STATO_DIAGRAMMA))
                    .ForMember(dest => dest.IsAutomatico, opt => opt.MapFrom(src => src.IstanzaPassoFirma.CHA_AUTOMATICO == "1"))
                    .ForMember(dest => dest.ApplicaSegnaturaPermanente, opt => opt.MapFrom(src => src.IstanzaPassoFirma.CHA_POS_SEGNATURA))
                    .ForMember(dest => dest.PosizioneSegnaturaPermanente, opt => opt.MapFrom(src => src.IstanzaPassoFirma.VAR_POS_SEGNATURA));

                cfg.CreateMap<AnagraficaEventiEntity, Evento>()
                    .ForMember(dest => dest.IdEvento, opt => opt.MapFrom(src => src.ID_EVENTO))
                    .ForMember(dest => dest.CodiceAzione, opt => opt.MapFrom(src => src.VAR_COD_AZIONE))
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                    .ForMember(dest => dest.TipoEvento, opt => opt.MapFrom(src => src.CHA_TIPO_EVENTO))
                    .ForMember(dest => dest.Gruppo, opt => opt.MapFrom(src => src.GRUPPO));
            });

            this._mapper = configuration.CreateMapper();
        }


        protected class IstanzaProcessiFirmaQueryEntity
        {
            public IstanzaProcessoFirmaEntity IstanzaProcessoFirma { get; set; }
            public ProfileEntity Profile { get; set; }
            public RuoloProponenteEntity RuoloProponente { get; set; }
            public UtenteProponenteEntity UtenteProponente { get; set; }
            public IstanzaPassoFirmaEntity? IstanzaPassoFirma { get; set; }
            public ProcessoFirmaVisibilitaEntity? ProcessoFirmaVisibilita { get; set; }
        }

        protected class IstanzaProcessiFirmaResultEntity
        {
            public IstanzaProcessoFirmaEntity IstanzaProcessoFirma { get; set; }
            public DocumentoEntity Documento { get; set; }
            public RuoloProponenteEntity RuoloProponente { get; set; }
            public UtenteProponenteEntity UtenteProponente { get; set; }
            public List<IstanzaPassoFirmaEventoEntity>? istanzePassoDiFirma { get; set; }
            public bool IS_TRONCATO { get; set; }
        }

        protected class RuoloProponenteEntity
        {
            public long? ID_GROUP { get; set; }
            public string GROUP_ID { get; set; }
            public string GROUP_DESCRIPTION { get; set; }
            public long ID_CORR_GLOBALI_RUOLO { get; set; }
        }

        protected class UtenteProponenteEntity
        {
            public long? ID_USER { get; set; }
            public string USER_ID { get; set; }
            public string USER_DESCRIPTION { get; set; }
            public long ID_CORR_GLOBALI_UTENTE { get; set; }
        }

        protected class DocumentoEntity
        {
            public long? NUM_PROTO { get; set; }
            public string? VAR_PROF_OGGETTO { get; set; }
            public DateTime? DTA_ANNULLA { get; set; }
            public DateTime? DTA_PROTO { get; set; }
            public DateTime? CREATION_DATE { get; set; }
            public string? VAR_SEGNATURA_REPERTORIO { get; set; }
        }

        protected class IstanzaPassoFirmaEventoEntity
        {
            public IstanzaPassoFirmaEntity IstanzaPassoFirma { get; set; }
            public AnagraficaEventiEntity Evento { get; set; }
        }
        #endregion
    }
}
