// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProspettiRiepilogativi;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using salvaModelloRequest = Pi3.App.Legacy.WebApi.Application.Requests.salvaModello;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.salvaModello
{
    public class salvaModelloHandler : IRequestHandler<salvaModelloRequest, salvaModelloResult>
    {

        protected readonly ILogger<salvaModelloHandler> _logger;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;


        private async Task<string> salvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
        {
            string dbID_MittDest = string.Empty;
            bool notificheUtImpostate = false;
            string result = string.Empty;

            if (Convert.ToString(modelloTrasmissione.SYSTEM_ID).Equals("0"))
            {
                ModelloTrasmEntity modelloTrasmEntity = new ModelloTrasmEntity()
                {
                    ID_AMM = modelloTrasmissione.ID_AMM.AsLong(),
                    NOME = modelloTrasmissione.NOME.Replace("'", "''"),
                    CHA_TIPO_OGGETTO = modelloTrasmissione.CHA_TIPO_OGGETTO,
                    ID_REGISTRO = string.IsNullOrEmpty(modelloTrasmissione.ID_REGISTRO) ? 0 : modelloTrasmissione.ID_REGISTRO.AsLong(),
                    VAR_NOTE_GENERALI = !string.IsNullOrEmpty(modelloTrasmissione.VAR_NOTE_GENERALI) ? modelloTrasmissione.VAR_NOTE_GENERALI.Replace("'", "''") : modelloTrasmissione.VAR_NOTE_GENERALI,
                    SINGLE = modelloTrasmissione.SINGLE,
                    ID_PEOPLE = string.IsNullOrEmpty(modelloTrasmissione.ID_PEOPLE) ? null: modelloTrasmissione.ID_PEOPLE.AsLong(),
                    CHA_CEDE_DIRITTI = string.IsNullOrEmpty(modelloTrasmissione.CEDE_DIRITTI) ? "0" : modelloTrasmissione.CEDE_DIRITTI,
                    ID_PEOPLE_NEW_OWNER = string.IsNullOrEmpty(modelloTrasmissione.ID_PEOPLE_NEW_OWNER) ? null : modelloTrasmissione.ID_PEOPLE_NEW_OWNER.AsLong(),
                    ID_GROUP_NEW_OWNER = string.IsNullOrEmpty(modelloTrasmissione.ID_GROUP_NEW_OWNER) ? null : modelloTrasmissione.ID_GROUP_NEW_OWNER.AsLong(),
                    NO_NOTIFY = modelloTrasmissione.NO_NOTIFY,
                    CHA_MANTIENI_LETTURA = string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_LETTURA) ? "0" : modelloTrasmissione.MANTIENI_LETTURA,
                    CHA_MANTIENI_SCRITTURA = string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_SCRITTURA) ? "0" : modelloTrasmissione.MANTIENI_SCRITTURA,
                };

                await this._dbContext.ModelloTrasmEntities.AddAsync(modelloTrasmEntity);

                int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();

                if(rows_affected > 0)
                {
                    result = "I^" +modelloTrasmEntity.SYSTEM_ID;
                    modelloTrasmissione.SYSTEM_ID = Convert.ToInt32(modelloTrasmEntity.SYSTEM_ID);

                }
                await this.insertSalvaModello(modelloTrasmissione);

            }
            else
            {
                int rowsAffected;

                ModelloTrasmEntity? modelToModify = await this._dbContext.ModelloTrasmEntities.FirstOrDefaultAsync(model => model.SYSTEM_ID == modelloTrasmissione.SYSTEM_ID);

                modelToModify.ID_AMM = modelloTrasmissione.ID_AMM.AsLong();
                modelToModify.NOME = modelloTrasmissione.NOME.Replace("'", "''");
                modelToModify.CHA_TIPO_OGGETTO = modelloTrasmissione.CHA_TIPO_OGGETTO;
                modelToModify.ID_REGISTRO = string.IsNullOrEmpty(modelloTrasmissione.ID_REGISTRO) ? 0 : modelloTrasmissione.ID_REGISTRO.AsLong();
                modelToModify.VAR_NOTE_GENERALI = string.IsNullOrEmpty(modelloTrasmissione.VAR_NOTE_GENERALI) ? null: modelloTrasmissione.VAR_NOTE_GENERALI.Replace("'", "''");
                modelToModify.SINGLE = modelloTrasmissione.SINGLE;
                modelToModify.ID_PEOPLE = string.IsNullOrEmpty(modelloTrasmissione.ID_PEOPLE) ? null : modelloTrasmissione.ID_PEOPLE.AsLong();
                modelToModify.CHA_CEDE_DIRITTI = string.IsNullOrEmpty(modelloTrasmissione.CEDE_DIRITTI) ? "" : modelloTrasmissione.CEDE_DIRITTI;
                modelToModify.ID_PEOPLE_NEW_OWNER = string.IsNullOrEmpty(modelloTrasmissione.ID_PEOPLE_NEW_OWNER) ? null : modelloTrasmissione.ID_PEOPLE_NEW_OWNER.AsLong();
                modelToModify.ID_GROUP_NEW_OWNER = string.IsNullOrEmpty(modelloTrasmissione.ID_GROUP_NEW_OWNER) ? null : modelloTrasmissione.ID_GROUP_NEW_OWNER.AsLong();
                modelToModify.NO_NOTIFY = modelloTrasmissione.NO_NOTIFY;
                modelToModify.CHA_MANTIENI_LETTURA = string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_LETTURA) ? "0" : modelloTrasmissione.MANTIENI_LETTURA;
                modelToModify.CHA_MANTIENI_SCRITTURA = string.IsNullOrEmpty(modelloTrasmissione.MANTIENI_SCRITTURA) ? "0" : modelloTrasmissione.MANTIENI_SCRITTURA;
                
                rowsAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                if (rowsAffected>=0)
                {
                    var listForDel = await this._dbContext.ModelloDestConNotificaEntities.
                     Where(row => row.ID_MODELLO == modelloTrasmissione.SYSTEM_ID).ToListAsync();

                    foreach (var el in listForDel)
                    {
                        ((DbContext)_dbContext).Remove(el);
                    }
                    ((DbContext)_dbContext).SaveChanges();


                    var entsMitt = await this._dbContext.ModelloMittDestEntities.Where(e => e.ID_MODELLO == modelloTrasmissione.SYSTEM_ID).ToListAsync();
                    foreach(var entMitt in entsMitt)
                    {
                        ((DbContext)_dbContext).Remove(entMitt);
                        ((DbContext)_dbContext).SaveChanges();
                    }


                    result = "U^" + modelloTrasmissione.SYSTEM_ID.ToString();


                    if (modelloTrasmissione.SINGLE != "1")
                    {
                        for (int j = 0; j < modelloTrasmissione.MITTENTE.Count(); j++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest Mittente = (DocsPaVO.Modelli_Trasmissioni.MittDest)modelloTrasmissione.MITTENTE[j];
                            ModelloMittDestEntity modelloMittDestEntity = new ModelloMittDestEntity()
                            {
                                ID_MODELLO = modelloTrasmissione.SYSTEM_ID,
                                CHA_TIPO_MITT_DEST = Mittente.CHA_TIPO_MITT_DEST,
                                ID_CORR_GLOBALI = Mittente.ID_CORR_GLOBALI,
                                ID_RAGIONE = Mittente.ID_RAGIONE,
                                CHA_TIPO_TRASM = Mittente.CHA_TIPO_TRASM,
                                VAR_NOTE_SING = string.IsNullOrWhiteSpace(Mittente.VAR_NOTE_SING) ? Mittente.VAR_NOTE_SING : Mittente.VAR_NOTE_SING.Replace("'", "''"),
                                CHA_TIPO_URP = Mittente.CHA_TIPO_URP,
                                SCADENZA = Mittente.SCADENZA,
                                HIDE_DOC_VERSIONS = Mittente.NASCONDI_VERSIONI_PRECEDENTI ? "1" : null

                            };

                            await this._dbContext.ModelloMittDestEntities.AddAsync(modelloMittDestEntity);

                            int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();
                        }
                    }

                    for (int i = 0; i < modelloTrasmissione.RAGIONI_DESTINATARI.Count(); i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.RagioneDest RagDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modelloTrasmissione.RAGIONI_DESTINATARI[i];
                        for (int l = 0; l < RagDest.DESTINATARI.Count(); l++)
                        {
                            DocsPaVO.Modelli_Trasmissioni.MittDest MittenteDestinatario = (DocsPaVO.Modelli_Trasmissioni.MittDest)RagDest.DESTINATARI[l];
                            ModelloMittDestEntity modelloMittDestEntity = new ModelloMittDestEntity()
                            {
                                ID_MODELLO = modelloTrasmissione.SYSTEM_ID,
                                CHA_TIPO_MITT_DEST = MittenteDestinatario.CHA_TIPO_MITT_DEST,
                                ID_CORR_GLOBALI = MittenteDestinatario.ID_CORR_GLOBALI,
                                ID_RAGIONE = MittenteDestinatario.ID_RAGIONE,
                                CHA_TIPO_TRASM = MittenteDestinatario.CHA_TIPO_TRASM,
                                VAR_NOTE_SING = string.IsNullOrWhiteSpace(MittenteDestinatario.VAR_NOTE_SING) ? MittenteDestinatario.VAR_NOTE_SING : MittenteDestinatario.VAR_NOTE_SING.Replace("'", "''"),
                                CHA_TIPO_URP = MittenteDestinatario.CHA_TIPO_URP,
                                SCADENZA = MittenteDestinatario.SCADENZA,
                                HIDE_DOC_VERSIONS = MittenteDestinatario.NASCONDI_VERSIONI_PRECEDENTI ? "1" : null

                            };

                            await this._dbContext.ModelloMittDestEntities.AddAsync(modelloMittDestEntity);

                            int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();

                            if( rows_affected > 0)
                            {
                                MittenteDestinatario.SYSTEM_ID = Convert.ToInt32(modelloMittDestEntity.SYSTEM_ID);
                                MittenteDestinatario.ID_MODELLO = modelloTrasmissione.SYSTEM_ID;
                                RagDest.DESTINATARI[l] = MittenteDestinatario;
                            }
                        }
                        modelloTrasmissione.RAGIONI_DESTINATARI[i] = RagDest;

                    }
                }
            }
            modelloTrasmissione = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.UtentiConNotificaTrasm(modelloTrasmissione, null, null, "SET"))).output;

            return result; 



        }

        private async Task insertSalvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
        {
            bool notificheUtImpostate = false;
            int rowsAffected;

            if (!modelloTrasmissione.SINGLE.Equals("1"))
            {
                for (int j = 0; j < modelloTrasmissione.MITTENTE.Count(); j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest Mittente = (DocsPaVO.Modelli_Trasmissioni.MittDest)modelloTrasmissione.MITTENTE[j];
                    ModelloMittDestEntity modelloMittDestEntity = new ModelloMittDestEntity()
                    {
                        ID_MODELLO = modelloTrasmissione.SYSTEM_ID,
                        CHA_TIPO_MITT_DEST = Mittente.CHA_TIPO_MITT_DEST,
                        ID_CORR_GLOBALI = Mittente.ID_CORR_GLOBALI,
                        ID_RAGIONE = Mittente.ID_RAGIONE,
                        CHA_TIPO_TRASM = Mittente.CHA_TIPO_TRASM,
                        VAR_NOTE_SING = string.IsNullOrWhiteSpace(Mittente.VAR_NOTE_SING) ? Mittente.VAR_NOTE_SING : Mittente.VAR_NOTE_SING.Replace("'", "''"),
                        CHA_TIPO_URP = Mittente.CHA_TIPO_URP,
                        SCADENZA = Mittente.SCADENZA,
                        HIDE_DOC_VERSIONS = Mittente.NASCONDI_VERSIONI_PRECEDENTI ? "1" : null

                    };

                    await this._dbContext.ModelloMittDestEntities.AddAsync(modelloMittDestEntity);

                    int rows_affected = await((DbContext)this._dbContext).SaveChangesAsync();



                }
            }


            for (int i = 0; i < modelloTrasmissione.RAGIONI_DESTINATARI.Count(); i++)
            {
                DocsPaVO.Modelli_Trasmissioni.RagioneDest RagDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modelloTrasmissione.RAGIONI_DESTINATARI[i];
                for (int l = 0; l < RagDest.DESTINATARI.Count(); l++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest MittenteDestinatario = (DocsPaVO.Modelli_Trasmissioni.MittDest)RagDest.DESTINATARI[l];
                    ModelloMittDestEntity modelloMittDestEntity = new ModelloMittDestEntity()
                    {
                        ID_MODELLO = modelloTrasmissione.SYSTEM_ID,
                        CHA_TIPO_MITT_DEST = MittenteDestinatario.CHA_TIPO_MITT_DEST,
                        ID_CORR_GLOBALI = MittenteDestinatario.ID_CORR_GLOBALI,
                        ID_RAGIONE = MittenteDestinatario.ID_RAGIONE,
                        CHA_TIPO_TRASM = MittenteDestinatario.CHA_TIPO_TRASM,
                        VAR_NOTE_SING = string.IsNullOrWhiteSpace(MittenteDestinatario.VAR_NOTE_SING) ? MittenteDestinatario.VAR_NOTE_SING : MittenteDestinatario.VAR_NOTE_SING.Replace("'", "''"),
                        CHA_TIPO_URP = MittenteDestinatario.CHA_TIPO_URP,
                        SCADENZA = MittenteDestinatario.SCADENZA,
                        HIDE_DOC_VERSIONS = MittenteDestinatario.NASCONDI_VERSIONI_PRECEDENTI ? "1" : null

                    };

                    await this._dbContext.ModelloMittDestEntities.AddAsync(modelloMittDestEntity);

                    int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();

                    if( rows_affected > 0)
                    {
                        MittenteDestinatario.SYSTEM_ID = Convert.ToInt32(modelloMittDestEntity.SYSTEM_ID);
                        MittenteDestinatario.ID_MODELLO = Convert.ToInt32(modelloTrasmissione.SYSTEM_ID.ToString());
                        RagDest.DESTINATARI[l] = MittenteDestinatario;

                        notificheUtImpostate = (MittenteDestinatario.UTENTI_NOTIFICA != null);
                    }
                }
                modelloTrasmissione.RAGIONI_DESTINATARI[i] = RagDest;

            }
            if (notificheUtImpostate)
                modelloTrasmissione = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.UtentiConNotificaTrasm(modelloTrasmissione, null, null, "SET"))).output;

        }


        public salvaModelloHandler(
            ILogger<salvaModelloHandler> logger,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IMediator mediator
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._webMethodLoggerService = webMethodLoggerService;
            this._mediator = mediator;
        }


        public async Task<salvaModelloResult> Handle(salvaModelloRequest request, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            try
            {
                result = await this.salvaModello(request.modelloTrasmissione);
                await this._webMethodLoggerService.LogOK("TRASMISSIONEINSMOD", request.modelloTrasmissione.SYSTEM_ID.ToString(), string.Format(Resource.LogInserimentoModello, request.modelloTrasmissione.NOME, request.infoUtente.userId));
            }
            catch ( Exception ex )
            {
                this._logger.LogWebMethodError(ex);
                await this._webMethodLoggerService.LogKO("TRASMISSIONEINSMOD",request.modelloTrasmissione.SYSTEM_ID.ToString(),string.Format(Resource.LogInserimentoModello,request.modelloTrasmissione.NOME,request.infoUtente.userId));
            }
            return new salvaModelloResult(result);
        }
            
    }
}
