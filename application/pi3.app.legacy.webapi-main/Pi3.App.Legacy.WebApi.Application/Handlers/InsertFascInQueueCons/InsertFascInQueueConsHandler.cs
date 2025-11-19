// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Conservazione.PARER;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsertFascInQueueConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertFascInQueueCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertFascInQueueCons
{
    public class InsertFascInQueueConsHandler : IRequestHandler<InsertFascInQueueConsRequest, InsertFascInQueueConsResult>
    {
        protected readonly ILogger<InsertFascInQueueConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;


        private async Task<string> GetStatoConsFasc(string idProject)
        {
            string? retVal = string.Empty;

            retVal = await this._dbContext.VersamentoFascicoliEntities.AsNoTracking().Where(ve => ve.ID_PROJECT == idProject.AsLong()).Select(ve => ve.CHA_STATO).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(retVal))
                retVal = "N";
            return retVal;
        }

        private async Task<bool> AddFascToQueueCons(string idProject, InfoUtente utente, string ente, string struttura)
        {
            bool result = false;
            long idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser,true);
            long idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup,true);
            long idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            try
            {
                VersamentoFascicoliEntity versamentoFascicolo = new()
                {
                    ID_PROJECT = idProject.AsLong(),
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = idGroup,
                    ID_AMM = idAmm,
                    CHA_STATO = "V",
                    VAR_CUSTOM_ENTE = ente ?? string.Empty,
                    VAR_CUSTOM_STRUTTURA = struttura ?? string.Empty,
                    VAR_FILE_RISPOSTA = null
                };

                this._dbContext.VersamentoFascicoliEntities.Add(versamentoFascicolo);
                int rowsAff = await ((DbContext)this._dbContext).SaveChangesAsync();
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
                this._logger.LogDebug(ex.Message);
            }
            return result;
        }

        private async Task<bool> UpdateQueueCons(string idProject, string stato, InfoUtente infoUtente, string warning, bool dataInvio, string numTentativo)
        {
            bool result = false;
            long idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            long idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            long idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            try
            {
                List<VersamentoFascicoliEntity>? fasc = await this._dbContext.VersamentoFascicoliEntities
                    .Where(ve => ve.ID_PROJECT == idProject.AsLong())
                    .ToListAsync();
                
                if(fasc != null)
                {
                    foreach (var fascToUpdate in fasc)
                    { 
                        fascToUpdate.DTA_INVIO = dataInvio ? await this._dbContext.GetSystemDateTime() : null;
                        fascToUpdate.CHA_STATO = stato;
                        if (infoUtente != null)
                        {
                            fascToUpdate.ID_PEOPLE = idPeople;
                            fascToUpdate.ID_RUOLO = idGroup;
                        }

                        fascToUpdate.VAR_FILE_RISPOSTA = string.Empty;
                        fascToUpdate.CHA_WARNING = warning;
                        fascToUpdate.NUM_TENTATIVI_INVIO = !string.IsNullOrEmpty(numTentativo) ? numTentativo.AsLong() : null;
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                }
            }
            catch (Exception ex)
            {
                result = false;
                this._logger.LogDebug(ex.Message);
                await transaction.RollbackAsync();
            }
            return result;
        }

        private async Task<bool> UpdateQueueSetCustomParams(string idProject, string ente, string struttura)
        {
            bool result = false;


            try
            {
                List<VersamentoFascicoliEntity>? versamenti = await this._dbContext.VersamentoFascicoliEntities.Where( ve => ve.ID_PROJECT == idProject.AsLong()).ToListAsync();

                if(versamenti != null)
                {
                    versamenti.ForEach((ve)=>
                    {
                        ve.VAR_CUSTOM_ENTE = ente;
                        ve.VAR_CUSTOM_STRUTTURA = struttura;
                    });

                    await ((DbContext)this._dbContext).SaveChangesAsync();
                    result = true;
                }
            }
            catch(Exception ex)
            {
                result = false;
                this._logger.LogDebug(ex.Message);
            }

            return result;
        }

        private async Task<string> GetIdRoleResponsabileConservazione(string idAmm, string idAOO)
        {
            string? result = string.Empty;


            try
            {
                if (!string.IsNullOrEmpty(idAOO))
                {
                    result = await this._dbContext.RespConsAooEntities.AsNoTracking().Where(rc => rc.ID_AMM == idAmm.AsLong() && rc.ID_REGISTRO == idAOO.AsLong()).Select(rc => rc.ID_GRUPPO_RESP_CONS.ToString()).FirstOrDefaultAsync();
                }
                else
                {
                    result = await this._dbContext.RespConsAooEntities.AsNoTracking().Where(rc => rc.ID_AMM == idAmm.AsLong() ).Select(rc => rc.ID_GRUPPO_RESP_CONS.ToString()).FirstOrDefaultAsync();
                }

            }
            catch(Exception ex)
            {
                this._logger.LogDebug(ex.Message);
            }
            return result;

        }

        private async Task<string> InsertFascInCons(string idProject, InfoUtente utente, string ente, string struttura)
        {
            string result = string.Empty;
            string? idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            try
            {
                string stato = await this.GetStatoConsFasc(idProject);

                if (stato.Equals("N") || stato.Equals("R") || stato.Equals("F"))
                {
                    if (stato == StatoVersamento.NUOVO)
                    {
                        result = await this.AddFascToQueueCons(idProject, utente, ente, struttura) ? "OK" : "INS_ERR";
                    }
                    else
                    {
                        if (!await this.UpdateQueueCons(idProject, StatoVersamento.IN_ATTESA, utente, string.Empty, false, "NULL"))
                        {
                            result = "INS_ERR";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ente) || !string.IsNullOrEmpty(struttura))
                            {
                                if (!await this.UpdateQueueSetCustomParams(idProject, ente, struttura))
                                {
                                    result = "UPD_ERR";
                                }
                                else
                                {
                                    result = "OK";
                                }
                            }
                            else
                            {
                                result = "OK";
                            }
                        }
                    }


                    string idAOO = string.Empty;
                    string? versKey = await this._configurationService.GetValue<string>(idAmm, "BE_VERSAMENTO_MULTI_AOO");
                    if (!string.IsNullOrEmpty(versKey) && versKey.Equals("1"))
                    {
                        var fascicolo = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoloByIdNoSecurity(idProject))).output;

                        if (fascicolo != null)
                        {
                            idAOO = fascicolo.idRegistroNodoTit;
                        }

                    }

                    if (result.Equals("OK"))
                    {
                        string idRuoloResp = await this.GetIdRoleResponsabileConservazione(utente.idAmministrazione, idAOO);
                        if (!string.IsNullOrEmpty(idRuoloResp))
                        {
                            this.AggiornaVisibilita(idProject, idRuoloResp, utente.idGruppo);
                        }
                    }
                }
                else
                {
                    // I fascicoli in stato ERRORE NELL'INVIO sono considerati in coda di versamento fin quando esauriscono il numero di tentativi di invio concessi
                    if (stato.Equals("V") || stato.Equals("T") || stato.Equals("W") || stato.Equals("E"))
                        result = "IN_QUEUE";
                    else if (stato.Equals("C"))
                        result = "FASC_CONS";
                    else
                        result = "STATE_ERR";
                }
            }
            
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                result = "CONS_ERR";
            }

            return result;
        }

        private async Task AggiornaVisibilita(string idProject, string idRuolo, string idUtente)
        {
            var check = await this.VerificaDirittiInSecurity(idProject, idRuolo);
            if (!string.IsNullOrWhiteSpace(check))
            {
                if (check == "INSERT") { await this.InsertInSec(idProject, idRuolo, idUtente); }
                else if (check == "UPDATE") { await this.UpdateSecurityCons(idProject, idRuolo); }
                else if (check == "OK") { this._logger.LogDebug("diritti già presenti"); }
            }
        }

        private async Task<bool> UpdateSecurityCons(string idDoc, string idGroup)
        {
            bool result = false;

            try
            {
                List<SecurityEntity>? secToModify = await this._dbContext.SecurityEntities.Where( s => s.THING == idDoc.AsLong() && s.PERSONORGROUP == idGroup.AsLong()).ToListAsync();
                
                if(secToModify != null)
                {
                    secToModify.ForEach((s) =>
                    {
                        s.ACCESSRIGHTS = 63;
                    });
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
                
                result = true;
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex.Message);
            }
            return result;
        }

        private async Task<bool> InsertInSec(string idDoc, string idGroup, string idVersatore)
        {
            bool result = false;
            try
            {
                SecurityEntity sec = new()
                {
                    THING = idDoc.AsLong(),
                    PERSONORGROUP = idGroup.AsLong(),
                    ACCESSRIGHTS = 63,
                    ID_GRUPPO_TRASM = idVersatore.AsLong(),
                    CHA_TIPO_DIRITTO = "C"
                };

                this._dbContext.SecurityEntities.Add(sec);

                await ((DbContext)this._dbContext).SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex.Message);
            }
            return result;
        }

        private async Task<string> VerificaDirittiInSecurity(string idDoc, string idGroup)
        {
            string result = string.Empty;

            try
            {
                var sec = await this._dbContext.SecurityEntities.AsNoTracking().Where(s => s.THING == idDoc.AsLong() && s.PERSONORGROUP == idGroup.AsLong()).MaxAsync(s => s.ACCESSRIGHTS);
                
                if(sec != null)
                {
                    var val = sec.ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        int rights = Convert.ToInt32(val);

                        // Se minore di 63 devo aggiornare la security per garantire diritti di lettura/scrittura
                        // Altrimenti non tocco nulla
                        if (rights < 63)
                            result = "UPDATE";
                        else
                            result = "OK";
                    }
                    else
                    {
                        // Record non presente
                        // Devo inserirlo nella security
                        result = "INSERT";
                    }
                }
                

            }
            catch(Exception ex)
            {
                this._logger.LogDebug(ex.Message);
            }
            return result;
        }

        public InsertFascInQueueConsHandler(
            ILogger<InsertFascInQueueConsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
             IMediator mediator
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
            this._mediator = mediator;
        }

        public async Task<InsertFascInQueueConsResult> Handle(InsertFascInQueueConsRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;
            try
            {
                output = await this.InsertFascInCons(request.idFasc,request.utente,string.Empty,string.Empty);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
