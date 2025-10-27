// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopiaProcessiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.CopiaProcessiFirma;
using CopiaProcessiFirmaHanlderResult = Pi3.App.Legacy.WebApi.Application.Requests.CopiaProcessiFirmaResult;
using GetFunzioniRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFunzioniRuolo;
using DuplicaProcessoFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DuplicaProcessoFirma;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using DocsPaVO.utente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CopiaProcessiFirma
{
    public class CopiaProcessiFirmaHandler : IRequestHandler<CopiaProcessiFirmaRequest, CopiaProcessiFirmaHanlderResult>
    {
        #region Public Members

        public CopiaProcessiFirmaHandler(ILogger<CopiaProcessiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<CopiaProcessiFirmaHanlderResult> Handle(CopiaProcessiFirmaRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.LibroFirma.CopiaProcessiFirmaResult> output = new List<DocsPaVO.LibroFirma.CopiaProcessiFirmaResult>();
            ResultProcessoFirma resultCreazioneProcesso = ResultProcessoFirma.KO; 
            try
            {
                var idRuoloDestAsLong = request.idRuoloDest.AsLong();
                long? idPeopleDestAsLong = !string.IsNullOrEmpty(request.idPeopleDest) ? request.idPeopleDest.AsLong() : null;
                var idCorrGlobaliRuoloDest = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_GRUPPO == idRuoloDestAsLong)
                    .Select(c => c.SYSTEM_ID)
                    .FirstAsync();

                var funzioniRuoloDest = (await this._mediator.Send(new GetFunzioniRuoloRequest(idCorrGlobaliRuoloDest.ToString()))).funzioni;

                if (!request.mantieniInRuoloOrigine)
                {
                    //Sposto tutti i processi dal vecchio ruolo a nuovo ruolo.
                    foreach(var processo in request.processiFirma)
                    {
                        var checkAutorization = await CheckAuthorizationPasso(processo, funzioniRuoloDest);
                        resultCreazioneProcesso = checkAutorization;
                        if (checkAutorization == ResultProcessoFirma.OK)
                        {
                            var idProcessoAsLong = processo.idProcesso.AsLong();

                            var processoFirmaEntity = await this._dbContext.SchemaProcessoFirmaEntities.
                                Where(c => c.ID_PROCESSO == idProcessoAsLong)
                                .FirstAsync();

                            if(!await this._dbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                                .AnyAsync(p => p.NOME.ToUpper() == processoFirmaEntity.NOME.ToUpper() && p.RUOLO_AUTORE == idRuoloDestAsLong))
                            {
                                processoFirmaEntity.RUOLO_AUTORE = idRuoloDestAsLong;
                                processoFirmaEntity.UTENTE_AUTORE = idPeopleDestAsLong;

                                await ((DbContext)_dbContext).SaveChangesAsync();
                            }
                            else
                            {
                                resultCreazioneProcesso = ResultProcessoFirma.EXISTING_PROCESS_NAME;
                            }
                        }
                        output.Add(new DocsPaVO.LibroFirma.CopiaProcessiFirmaResult() { idProcesso = processo.idProcesso, nomeProcesso = processo.nome, esito = resultCreazioneProcesso });
                    }
                }
                else
                {
                    foreach (var processo in request.processiFirma)
                    {
                        var checkAutorization = await CheckAuthorizationPasso(processo, funzioniRuoloDest);
                        resultCreazioneProcesso = checkAutorization;
                        if (checkAutorization == ResultProcessoFirma.OK)
                        {

                            var duplicaProcessoFirmaResult = await this._mediator.Send(new DuplicaProcessoFirmaRequest(processo, processo.nome, request.copiaVisibilita,
                                new InfoUtente()
                                {
                                    idPeople = idPeopleDestAsLong.ToString(),
                                    idGruppo = idRuoloDestAsLong.ToString()
                                }));
                            resultCreazioneProcesso = duplicaProcessoFirmaResult.resultCreazioneProcesso;
                        }

                        output.Add(new DocsPaVO.LibroFirma.CopiaProcessiFirmaResult() { idProcesso = processo.idProcesso, nomeProcesso = processo.nome, esito = resultCreazioneProcesso });
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new CopiaProcessiFirmaHanlderResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CopiaProcessiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected async Task<ResultProcessoFirma> CheckAuthorizationPasso(ProcessoFirma processo, Funzione[] funzioniRuoloDest)
        {
            ResultProcessoFirma esito = ResultProcessoFirma.OK;

            try
            {
                //Controllo abilitazione del ruolo alla creazione dei modelli di firma
                if (processo.IsProcessModel)
                {
                    if (!funzioniRuoloDest.Any(f => f.codice.ToUpper().Equals("DO_CREATE_MODEL_PROCESS")))
                    {
                        return ResultProcessoFirma.RUOLO_NON_ABILITATO_A_CREAZIONE_MODELLI;
                    }
                }

                //Controllo dei passi automatici
                foreach (PassoFirma passo in processo.passi)
                {
                    if (passo.IsAutomatico)
                    {
                        Azione azione = (Azione)Enum.Parse(typeof(Azione), passo.Evento.CodiceAzione, true);
                        switch (azione)
                        {
                            case Azione.RECORD_PREDISPOSED:
                                if (!funzioniRuoloDest.Any(f => f.codice.ToUpper().Equals("CREA_PASSO_PROTO_AUTO")))
                                    esito = ResultProcessoFirma.RUOLO_NON_ABILITATO_A_CREAZIONE_PASSO_PROTO_AUTO;
                                break;
                            case Azione.DOCUMENTOSPEDISCI:
                                if (!funzioniRuoloDest.Any(f => f.codice.ToUpper().Equals("CREA_PASSO_SPEDIZIONE_AUTO")))
                                    esito = ResultProcessoFirma.RUOLO_NON_ABILITATO_A_CREAZIONE_PASSO_SPEDIZIONE_AUTO;
                                break;
                            case Azione.DOCUMENTO_REPERTORIATO:
                                if (!funzioniRuoloDest.Any(f => f.codice.ToUpper().Equals("CREA_PASSO_REPERTORIAZIONE_AUTO")))
                                    esito = ResultProcessoFirma.RUOLO_NON_ABILITATO_A_CREAZIONE_PASSO_REPO_AUTO;
                                break;
                        }
                    }
                    if (esito != ResultProcessoFirma.OK)
                        return esito;
                }
            }
            catch (Exception e)
            {
                esito = ResultProcessoFirma.KO;
            }

            return esito;
        }

        #endregion
    }

}
