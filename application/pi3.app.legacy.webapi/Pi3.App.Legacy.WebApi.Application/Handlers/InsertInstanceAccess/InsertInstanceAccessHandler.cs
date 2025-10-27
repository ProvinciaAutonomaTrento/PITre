// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.InstanceAccess;
using DocsPaVO.ProspettiRiepilogativi;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using InsertInstanceAccessRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertInstanceAccess;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertInstanceAccess
{
    public class InsertInstanceAccessHandler : IRequestHandler<InsertInstanceAccessRequest, InsertInstanceAccessResult>
    {
        protected readonly ILogger<InsertInstanceAccessHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;



        private string CorrectApici(string str)
        {
            if (str != null)
            {
                return str.Replace("'", "''");
            }
            else
            {
                return str;
            }
        }


        private async Task<long> InsOcc2SP(long p_ID_REG,long p_IDAMM,string p_Prefix_cod_rub,string p_DESC_CORR,string p_CHA_DETTAGLI,string p_ID_Corr_Globali,string p_EMAIL )
        {
            int myprofile = 0;
            int countProfDoc = 0;
            int countProffAsc = 0;
            int countProfilato = 0;
            if (p_ID_Corr_Globali.AsLong() != 0)
            {
                myprofile = await this._dbContext.DocArrivoParEntities.AsNoTracking().Where(ent => ent.ID_MITT_DEST == p_ID_Corr_Globali.AsLong()).CountAsync();

                countProfDoc = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking().Where(e => e.VALORE_OGGETTO_DB == p_ID_Corr_Globali).CountAsync();

                countProffAsc = await this._dbContext.AssTemplatesFascEntities.AsNoTracking().Where(e => e.VALORE_OGGETTO_DB == p_ID_Corr_Globali).CountAsync();

                countProfilato = countProffAsc + countProfDoc;
            }
            long sysid = await ((DbContext)this._dbContext).Database.SqlQueryRaw<long>("select seq.nextval from dual;").FirstOrDefaultAsync();
            
            if (myprofile == 0 && countProfilato == 0)
            {
                if (p_ID_REG == 0)
                {
                    CorrGlobaliEntity corrGlobaliEntity = new CorrGlobaliEntity()
                    {
                        SYSTEM_ID = sysid,
                        ID_REGISTRO = null,
                        ID_AMM = p_IDAMM,
                        VAR_COD_RUBRICA = string.Concat(p_Prefix_cod_rub, sysid.ToString()),
                        VAR_DESC_CORR = p_DESC_CORR,
                        ID_OLD = 0,
                        DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                        ID_PARENT = 0,
                        VAR_CODICE = string.Concat(p_Prefix_cod_rub,sysid.ToString()),
                        CHA_TIPO_CORR = "O",
                        CHA_DETTAGLI = "0",
                        VAR_EMAIL = p_EMAIL

                    };


                    this._dbContext.CorrGlobaliEntities.Add(corrGlobaliEntity);
                    ((DbContext)this._dbContext).SaveChanges();
                }

                else
                {
                    CorrGlobaliEntity corrGlobaliEntity = new CorrGlobaliEntity()
                    {
                        SYSTEM_ID = sysid,
                        ID_REGISTRO = p_ID_REG,
                        ID_AMM = p_IDAMM,
                        VAR_COD_RUBRICA = string.Concat(p_Prefix_cod_rub, sysid),
                        VAR_DESC_CORR = p_DESC_CORR,
                        ID_OLD = 0,
                        DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                        ID_PARENT = 0,
                        VAR_CODICE = string.Concat(p_Prefix_cod_rub, sysid.ToString()),
                        CHA_TIPO_CORR = "O",
                        CHA_DETTAGLI = "0",
                        VAR_EMAIL = p_EMAIL

                    };

                    this._dbContext.CorrGlobaliEntities.Add(corrGlobaliEntity);
                    ((DbContext)this._dbContext).SaveChanges();
                }
            }

            else
            {
                var ent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where( corr => corr.SYSTEM_ID == p_ID_Corr_Globali.AsLong()).Select( e => new
                {
                    e.VAR_COD_RUBRICA,
                    e.ID_REGISTRO,
                    e.ID_AMM
                }).FirstOrDefaultAsync();


                if( ent != null)
                {

                    var new_var_cod_rubrica1 = ent.VAR_COD_RUBRICA;
                    var id_reg = ent.ID_REGISTRO;
                    var idamm = ent.ID_AMM;

                    new_var_cod_rubrica1 = string.Concat(string.Concat(new_var_cod_rubrica1,"_"), p_ID_Corr_Globali);

                    CorrGlobaliEntity? corrToUpdate = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == p_ID_Corr_Globali.AsLong()).FirstOrDefaultAsync();
                    if(corrToUpdate != null)
                    {
                        corrToUpdate.DTA_FINE = await this._dbContext.GetSystemDateTime();
                        corrToUpdate.VAR_COD_RUBRICA = new_var_cod_rubrica1;
                        corrToUpdate.VAR_CODICE = new_var_cod_rubrica1;
                        corrToUpdate.ID_PARENT = null;

                        
                        ((DbContext)this._dbContext).SaveChanges();
                    }

                    CorrGlobaliEntity corrToInsert = new()
                    {
                        SYSTEM_ID = sysid,
                        ID_REGISTRO = id_reg,
                        ID_AMM = idamm,
                        VAR_COD_RUBRICA = string.Concat(p_Prefix_cod_rub, sysid.ToString()),
                        VAR_DESC_CORR = p_DESC_CORR,
                        ID_OLD = p_ID_Corr_Globali.AsLong(),
                        DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                        ID_PARENT = 0,
                        VAR_CODICE = string.Concat(p_Prefix_cod_rub,sysid.ToString()),
                        CHA_TIPO_CORR = "O",
                        CHA_DETTAGLI = "0",
                        VAR_EMAIL = p_EMAIL

                    };

                    this._dbContext.CorrGlobaliEntities.Add(corrToInsert);
                    ((DbContext)this._dbContext).SaveChanges();

                }

            }
            return sysid;
        }






        private bool MakeTyped(DataSet source, DataSet destination)
        {
            bool result = true; 

            try
            {
                foreach (DataTable table in source.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        destination.Tables[0].ImportRow(row);
                    }
                }
            }
            catch (Exception exception)
            {
                this._logger.LogWebMethodError(exception);
                result = false;
            }

            return result;
        }


        private async Task<bool> CheckExistDettagliCorr(string idCorr)
        {
            bool result = false;
            try
            {
                int dettGlCount = await this._dbContext.DettGlobaliEntities.AsNoTracking().Where(det => det.ID_CORR_GLOBALI == idCorr.AsLong()).CountAsync();
                
                result = (dettGlCount != 0);
                

            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                result = false;
            }
            return result;
        }

        private async Task<InstanceAccess> InsertInstanceAccess(InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();

            try
            {
                if (instanceAccess != null)
                {
                    if (instanceAccess.RICHIEDENTE != null && instanceAccess.RICHIEDENTE.tipoCorrispondente.Equals("O") && string.IsNullOrEmpty(instanceAccess.RICHIEDENTE.systemId))
                    {
                        string prefix = await this._configurationService.GetValue<string>("prefissoCorrOccasionale");
                        long resultStore = await this.InsOcc2SP(0, infoUtente.idAmministrazione.AsLong(), prefix, instanceAccess.RICHIEDENTE.descrizione, "0", "0", null);

                        if (resultStore != -1 && resultStore != 0)
                        {
                            instanceAccess.RICHIEDENTE.systemId = resultStore.ToString();
                            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                            this.MakeTyped(instanceAccess.RICHIEDENTE.info, dettagliCorrispondente.Corrispondente.DataSet);

                            if (!await this.CheckExistDettagliCorr(instanceAccess.RICHIEDENTE.systemId))
                            {

                                DettGlobaliEntity dettGlobali = new()
                                {
                                    ID_CORR_GLOBALI = instanceAccess.RICHIEDENTE.systemId.AsLong(),
                                    VAR_INDIRIZZO = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].indirizzo),
                                    VAR_CAP = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].cap),
                                    VAR_CITTA = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].citta),
                                    VAR_PROVINCIA = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].provincia),
                                    VAR_NAZIONE = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].nazione),
                                    VAR_TELEFONO = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].telefono),
                                    VAR_TELEFONO2 = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].telefono2),
                                    VAR_FAX = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].fax),
                                    VAR_COD_FISC = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].codiceFiscale),
                                    VAR_NOTE = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].note),
                                    VAR_LOCALITA = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].localita),
                                    VAR_LUOGO_NASCITA = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].luogoNascita),
                                    DTA_NASCITA = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].dataNascita),
                                    VAR_TITOLO = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].titolo),
                                    VAR_COD_PI = this.CorrectApici(dettagliCorrispondente.Corrispondente[0].partitaIva)
                                };

                                this._dbContext.DettGlobaliEntities.Add(dettGlobali);
                                await ((DbContext)this._dbContext).SaveChangesAsync();
                            }
                        }
                        else
                        {
                            return null;
                        }

                    }
                    InstanceAccessEntity? entity = new()
                    {
                        DESCRIPTION = instanceAccess.DESCRIPTION.Replace("'", "''"),
                        DTA_CREAZIONE = instanceAccess.CREATION_DATE,
                        DTA_CHIUSURA = null,
                        ID_PEOPLE_PROPRIETARIO = instanceAccess.ID_PEOPLE_OWNER.AsLong(),
                        ID_GRUPPO_PROPRIETARIO = instanceAccess.ID_GROUPS_OWNER.AsLong(),
                        ID_RICHIEDENTE = instanceAccess.RICHIEDENTE == null ? null : instanceAccess.RICHIEDENTE.systemId.AsLong(),
                        DTA_RICHIESTA = instanceAccess.REQUEST_DATE,
                        ID_DOCUMENTO_RICHIESTO = !string.IsNullOrEmpty(instanceAccess.ID_DOCUMENT_REQUEST) ? instanceAccess.ID_DOCUMENT_REQUEST.AsLong() : null,
                        NOTE = instanceAccess.NOTE.Replace("'", "''"),
                        CHA_STATO_DOWNLOAD_INOLTRO = "0",
                        ID_PROFILE_DOWNLOAD = 0
                    };

                    this._dbContext.InstanceAccessEntities.Add(entity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (entity != null && entity.SYSTEM_ID != null)
                    {
                        instanceAccess.ID_INSTANCE_ACCESS = entity.SYSTEM_ID.ToString();
                        instanceAccess.DOCUMENTS = new();
                    }
                    else
                    {
                        instanceAccess.ID_INSTANCE_ACCESS = null;
                    }


                }
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
            await transaction.CommitAsync();


            return instanceAccess;

        }
        public InsertInstanceAccessHandler(
        ILogger<InsertInstanceAccessHandler> logger,
        IPi3DbContext dbContext,
        IConfigurationService configurationService
        )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._configurationService = configurationService;
        }


        public async Task<InsertInstanceAccessResult> Handle(InsertInstanceAccessRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.InstanceAccess.InstanceAccess output = null;

            try
            {
                output = await this.InsertInstanceAccess(request.instanceAccess, request.infoUtente);
            }
            catch(Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);

            }
            return new(output);
        }
    }
}
