// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.IsCodRubricaPresente;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookInsertCorrispondente
{
    public class AddressbookInsertCorrispondenteCommandHandler : IRequestHandler<AddressbookInsertCorrispondenteCommand, AddressbookInsertCorrispondenteCommandResponse>
    {
        public AddressbookInsertCorrispondenteCommandHandler(

            IWebMethodLoggerService webMethodLoggerService,
            ILogger<AddressbookInsertCorrispondenteCommandHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator,
            IConfigurationService configurationService
            )
        {
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._logger = logger;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<AddressbookInsertCorrispondenteCommandResponse> Handle(AddressbookInsertCorrispondenteCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Corrispondente? output = null;

            try
            {
                output = await this.InsertCorr(request.Corrispondente, request.Parent);

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new()
            {
                Output = output
            };
        }



        #region Private Members

        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ILogger<AddressbookInsertCorrispondenteCommandHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;



        private async Task<DocsPaVO.utente.Corrispondente> InsertCorr(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent)
        {
            DocsPaVO.utente.Corrispondente? result = new();

            if (corr.dettagli && corr.tipoCorrispondente != null && corr.tipoCorrispondente.ToUpper() != "R")
            {
                DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                if (corr.tipoCorrispondente.ToUpper().Equals("U"))
                {
                    this.TypedCp(((DocsPaVO.utente.UnitaOrganizzativa)corr).info, dettagliCorrispondente.Corrispondente.DataSet);
                }
                else if (corr.tipoCorrispondente.ToUpper().Equals("P"))
                {
                    this.TypedCp(((DocsPaVO.utente.Utente)corr).info, dettagliCorrispondente.Corrispondente.DataSet);
                }
                else if (corr.tipoCorrispondente.ToUpper().Equals("F"))
                {
                    this.TypedCp(((DocsPaVO.utente.RaggruppamentoFunzionale)corr).info, dettagliCorrispondente.Corrispondente.DataSet);
                }
                var msg = this.CheckValidity(dettagliCorrispondente, corr.tipoCorrispondente);
                if (!string.IsNullOrEmpty(msg))
                {
                    result.errore = msg;
                    return result;
                }
            }


            (bool insertSucc, DocsPaVO.utente.Corrispondente resIns) = await this.Insert(corr, parent, corr.idAmministrazione);
            if (insertSucc)
            {
                if (resIns != null && !string.IsNullOrEmpty(resIns.systemId) && corr.interoperanteRGS)
                {
                    CorrInteropEntity corrInteropEntity = new CorrInteropEntity()
                    {
                        ID_CORR = resIns.systemId.AsLong(),
                        CHA_INTEROPERANTE_RGS = "1"
                    };

                    this._dbContext.CorrInteropEntities.Add(corrInteropEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
            return resIns;

        }

        // returns ( isInsertSucc, CorrAdded )
        private async Task<(bool, DocsPaVO.utente.Corrispondente)> Insert(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent, string idAmm)
        {
            var conditions = new List<Expression<Func<CorrGlobaliEntity, bool>>>
                {
                    (c) => c.VAR_COD_RUBRICA != null ? c.VAR_COD_RUBRICA.ToUpper().Equals(corr.codiceRubrica.ToUpper()) : false
                };


            if (!string.IsNullOrEmpty(idAmm))
                conditions.Add((c) => (c.ID_AMM == null || c.ID_AMM == idAmm.AsLong()));

            if (!string.IsNullOrEmpty(corr.idRegistro))
                conditions.Add((c) =>
                (c.ID_REGISTRO != null ? c.ID_REGISTRO == corr.idRegistro.AsLong() : false) ||
                (c.ID_REGISTRO == null && (c.CHA_TIPO_IE != null ? c.CHA_TIPO_IE.Equals("I") : false)));
            else
                conditions.Add((c) => c.ID_REGISTRO == null);

            conditions.Add(c => (!c.DTA_FINE.HasValue));

            Expression<Func<CorrGlobaliEntity, bool>> predicate = BuildAndPredicate(conditions);

            var dataSet = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(predicate).Select(
                c => new
                {
                    c.SYSTEM_ID,
                    c.CHA_TIPO_IE,
                    c.CHA_TIPO_CORR
                }).ToListAsync();


            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            bool isInsertSucc = true;
            DocsPaVO.utente.Corrispondente corrToAdd = new();
            (string? insertPrefix, bool keyFound) = await this._configurationService.TryGetValue<string>(Resources.prefixKey);

            try
            {
                string ammId = corr.idAmministrazione;
                if (string.IsNullOrEmpty(ammId))
                    ammId = idAmm;

                try
                {
                    bool isCodRubPresente = (await this._mediator.Send(new IsCodRubricaPresenteCommand()
                    {
                        CodRubrica = CorrectApici(corr.codiceRubrica),
                        TipoCorr = corr.tipoCorrispondente, 
                        IdAmm = ammId, 
                        IdReg = corr.idRegistro, 
                        InRubricaComune = corr.inRubricaComune
                    })).Output;
                    bool isCodCorrect = await this.IsCodeCorrect(CorrectApici(corr.codiceRubrica));

                    if (!isCodCorrect || isCodRubPresente)
                    {
                        return (false, corrToAdd);
                    }
                }
                catch (Exception pi3Ex)
                {
                    corrToAdd.errore = pi3Ex.Message;
                    return (false, corrToAdd);
                }

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
                {
                    if (string.IsNullOrEmpty(corr.oldDescrizione))
                    {
                        corr.oldDescrizione = corr.descrizione;
                    }

                    CorrGlobaliEntity insertedCorr = (await this.InsertCorrUR(corr, parent));

                    if (string.IsNullOrEmpty(corr.codiceRubrica))
                    {
                        insertedCorr.VAR_COD_RUBRICA = insertPrefix + "U_" + insertedCorr.SYSTEM_ID.ToString();
                        insertedCorr.VAR_CODICE = insertPrefix + "U_" + insertedCorr.SYSTEM_ID.ToString();

                        try
                        {
                            await ((DbContext)this._dbContext).SaveChangesAsync();
                            corr.codiceRubrica = insertedCorr.VAR_COD_RUBRICA;
                        }
                        catch (Exception ex)
                        {
                            isInsertSucc = false;
                            await transaction.RollbackAsync();
                            this._logger.LogError(ex,ex.Message);
                        }
                    }

                    if (((DocsPaVO.utente.UnitaOrganizzativa)corr).info != null)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                        this.TypedCp(((DocsPaVO.utente.UnitaOrganizzativa)corr).info, dettagliCorrispondente.Corrispondente.DataSet);

                        DettGlobaliEntity dettEnt = new DettGlobaliEntity()
                        {
                            ID_CORR_GLOBALI = insertedCorr.SYSTEM_ID,
                            VAR_INDIRIZZO = (dettagliCorrispondente.Corrispondente[0].indirizzo),
                            VAR_CAP = (dettagliCorrispondente.Corrispondente[0].cap),
                            VAR_CITTA = (dettagliCorrispondente.Corrispondente[0].citta),
                            VAR_PROVINCIA = (dettagliCorrispondente.Corrispondente[0].provincia),
                            VAR_NAZIONE = (dettagliCorrispondente.Corrispondente[0].nazione),
                            VAR_TELEFONO = (dettagliCorrispondente.Corrispondente[0].telefono),
                            VAR_TELEFONO2 = (dettagliCorrispondente.Corrispondente[0].telefono2),
                            VAR_FAX = (dettagliCorrispondente.Corrispondente[0].fax),
                            VAR_COD_FISC = (dettagliCorrispondente.Corrispondente[0].codiceFiscale),
                            VAR_NOTE = (dettagliCorrispondente.Corrispondente[0].note),
                            VAR_LOCALITA = (dettagliCorrispondente.Corrispondente[0].localita),
                            VAR_LUOGO_NASCITA = (dettagliCorrispondente.Corrispondente[0].luogoNascita),
                            DTA_NASCITA = !string.IsNullOrEmpty(dettagliCorrispondente.Corrispondente[0].dataNascita) ? dettagliCorrispondente.Corrispondente[0].dataNascita : null,
                            VAR_TITOLO = (dettagliCorrispondente.Corrispondente[0].titolo),
                            VAR_COD_PI = (dettagliCorrispondente.Corrispondente[0].partitaIva)
                        };

                        this._dbContext.DettGlobaliEntities.Add(dettEnt);

                        try
                        {
                            await ((DbContext)this._dbContext).SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            isInsertSucc = false;
                            await transaction.RollbackAsync();
                            this._logger.LogError(ex, ex.Message);
                        }
                    }

                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                        {
                            corr.canalePref.systemId = (await this._dbContext.DocumentTypesEntities.AsNoTracking().
                                Where(t => t.TYPE_ID != null && t.TYPE_ID.Equals("LETTERA")).Select(t => t.SYSTEM_ID).FirstOrDefaultAsync()).ToString();

                        }
                        CanaleCorrEntity canaleCorrEntity = new()
                        {
                            ID_CORR_GLOBALE = insertedCorr.SYSTEM_ID,
                            ID_DOCUMENTTYPE = corr.canalePref.systemId.AsLong(),
                            CHA_PREFERITO = "1"
                        };

                        this._dbContext.CanaleCorrEntities.Add(canaleCorrEntity);
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }

                    if (!string.IsNullOrEmpty(insertedCorr.SYSTEM_ID.ToString()) && dataSet.Count > 0)
                    {
                        foreach (var r in dataSet)
                        {
                            string idCorrOld = r.SYSTEM_ID.ToString();
                            string idCorrNew = insertedCorr.SYSTEM_ID.ToString();
                            await this.AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }
                    corr.systemId = insertedCorr.SYSTEM_ID.ToString();
                }

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                {
                    CorrGlobaliEntity corrGlobaliEntity = new()
                    {
                        CHA_TIPO_IE = "E",
                        ID_REGISTRO = !string.IsNullOrEmpty(corr.idRegistro) ? corr.idRegistro.AsLong() : null,
                        ID_AMM = !string.IsNullOrEmpty(corr.idAmministrazione) ? corr.idAmministrazione.AsLong() : null,
                        VAR_DESC_CORR = (corr.descrizione) + " " + (((DocsPaVO.utente.UnitaOrganizzativa)parent).descrizione),
                        ID_OLD = 0,
                        DTA_INIZIO = await _dbContext.GetSystemDateTime(),
                        ID_UO = parent.systemId.AsLong(),
                        VAR_CODICE = (corr.codiceCorrispondente),
                        CHA_TIPO_CORR = "S",
                        CHA_TIPO_URP = "R",
                        VAR_COD_RUBRICA = !string.IsNullOrEmpty(corr.codiceRubrica) ? (corr.codiceRubrica) : string.Empty,
                        CHA_DETTAGLI = corr.dettagli ? "1" : "0",
                        VAR_EMAIL = corr.email,
                        VAR_CODICE_AMM = corr.codiceAmm,
                        VAR_CODICE_AOO = corr.codiceAOO,
                        CHA_PA = "1",
                        CHA_SYSTEM_ROLE = "0"
                    };
                    this._dbContext.CorrGlobaliEntities.Add(corrGlobaliEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        corrGlobaliEntity.VAR_CODICE = insertPrefix + "R_" + corrGlobaliEntity.SYSTEM_ID.ToString();
                        corrGlobaliEntity.VAR_COD_RUBRICA = insertPrefix + "R_" + corrGlobaliEntity.SYSTEM_ID.ToString();

                        try
                        {
                            await ((DbContext)this._dbContext).SaveChangesAsync();
                            corr.codiceRubrica = corrGlobaliEntity.VAR_CODICE;
                        }
                        catch (Exception ex)
                        {
                            isInsertSucc = false;
                            await transaction.RollbackAsync();
                            this._logger.LogError(ex, ex.Message);
                        }
                    }

                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                        {
                            corr.canalePref.systemId = (await this._dbContext.DocumentTypesEntities.AsNoTracking().
                                Where(t => t.TYPE_ID != null && t.TYPE_ID.Equals("LETTERA")).Select(t => t.SYSTEM_ID).FirstOrDefaultAsync()).ToString();
                        }
                        CanaleCorrEntity canaleCorrEntity = new()
                        {
                            ID_CORR_GLOBALE = corrGlobaliEntity.SYSTEM_ID,
                            ID_DOCUMENTTYPE = corr.canalePref.systemId.AsLong(),
                            CHA_PREFERITO = "1"
                        };

                        this._dbContext.CanaleCorrEntities.Add(canaleCorrEntity);
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }

                    if (!string.IsNullOrEmpty(corrGlobaliEntity.SYSTEM_ID.ToString()) && dataSet.Count > 0)
                    {
                        foreach (var r in dataSet)
                        {
                            string idCorrOld = r.SYSTEM_ID.ToString();
                            string idCorrNew = corrGlobaliEntity.SYSTEM_ID.ToString();
                            await this.AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }

                    corr.systemId = corrGlobaliEntity.SYSTEM_ID.ToString();
                }

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    string titolo = !string.IsNullOrEmpty(CorrectApici(((DocsPaVO.utente.Utente)corr).titolo)) ? CorrectApici(((DocsPaVO.utente.Utente)corr).titolo) + " " : string.Empty;
                    CorrGlobaliEntity utenteEnt = new()
                    {
                        CHA_TIPO_IE = "E",
                        ID_REGISTRO = !string.IsNullOrEmpty(corr.idRegistro) ? corr.idRegistro.AsLong() : null,
                        ID_AMM = !string.IsNullOrEmpty(corr.idAmministrazione) ? corr.idAmministrazione.AsLong() : null,
                        VAR_DESC_CORR = titolo + ((DocsPaVO.utente.Utente)corr).cognome + " " + (((DocsPaVO.utente.Utente)corr).nome),
                        ID_OLD = 0,
                        DTA_INIZIO = await _dbContext.GetSystemDateTime(),
                        VAR_CODICE = corr.codiceCorrispondente,
                        CHA_TIPO_CORR = "S",
                        CHA_TIPO_URP = "P",
                        VAR_COD_RUBRICA = !string.IsNullOrEmpty(corr.codiceRubrica) ? (corr.codiceRubrica) : string.Empty,
                        CHA_DETTAGLI = corr.dettagli ? "1" : "0",
                        VAR_EMAIL = ((DocsPaVO.utente.Utente)corr).email,
                        VAR_COGNOME = ((DocsPaVO.utente.Utente)corr).cognome,
                        VAR_NOME = ((DocsPaVO.utente.Utente)corr).nome,
                        VAR_CODICE_AMM = corr.codiceAmm,
                        VAR_CODICE_AOO = corr.codiceAOO,
                        CHA_PA = "1",
                        CHA_SYSTEM_ROLE = "0"
                    };

                    this._dbContext.CorrGlobaliEntities.Add(utenteEnt);

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        utenteEnt.VAR_CODICE = insertPrefix + "U_" + utenteEnt.SYSTEM_ID.ToString();
                        utenteEnt.VAR_COD_RUBRICA = insertPrefix + "U_" + utenteEnt.SYSTEM_ID.ToString();

                        try
                        {
                            await ((DbContext)this._dbContext).SaveChangesAsync();
                            corr.codiceRubrica = utenteEnt.VAR_COD_RUBRICA;
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, ex.Message);
                            await transaction.RollbackAsync();
                            isInsertSucc = false;
                        }
                    }

                    if (((DocsPaVO.utente.Utente)corr).info != null)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                        this.TypedCp(((DocsPaVO.utente.Utente)corr).info, dettagliCorrispondente.Corrispondente.DataSet);
                        DettGlobaliEntity dettEnt = new DettGlobaliEntity()
                        {
                            ID_CORR_GLOBALI = utenteEnt.SYSTEM_ID,
                            VAR_INDIRIZZO = CorrectApici(dettagliCorrispondente.Corrispondente[0].indirizzo),
                            VAR_CAP = CorrectApici(dettagliCorrispondente.Corrispondente[0].cap),
                            VAR_CITTA = CorrectApici(dettagliCorrispondente.Corrispondente[0].citta),
                            VAR_PROVINCIA = CorrectApici(dettagliCorrispondente.Corrispondente[0].provincia),
                            VAR_NAZIONE = CorrectApici(dettagliCorrispondente.Corrispondente[0].nazione),
                            VAR_TELEFONO = CorrectApici(dettagliCorrispondente.Corrispondente[0].telefono),
                            VAR_TELEFONO2 = CorrectApici(dettagliCorrispondente.Corrispondente[0].telefono2),
                            VAR_FAX = CorrectApici(dettagliCorrispondente.Corrispondente[0].fax),
                            VAR_COD_FISC = CorrectApici(dettagliCorrispondente.Corrispondente[0].codiceFiscale),
                            VAR_NOTE = CorrectApici(dettagliCorrispondente.Corrispondente[0].note),
                            VAR_LOCALITA = CorrectApici(dettagliCorrispondente.Corrispondente[0].localita),
                            VAR_LUOGO_NASCITA = CorrectApici(dettagliCorrispondente.Corrispondente[0].luogoNascita),
                            DTA_NASCITA = !string.IsNullOrEmpty(dettagliCorrispondente.Corrispondente[0].dataNascita) ? dettagliCorrispondente.Corrispondente[0].dataNascita : null,
                            VAR_TITOLO = CorrectApici(dettagliCorrispondente.Corrispondente[0].titolo),
                            VAR_COD_PI = CorrectApici(dettagliCorrispondente.Corrispondente[0].partitaIva)
                        };

                        this._dbContext.DettGlobaliEntities.Add(dettEnt);

                        try
                        {
                            await ((DbContext)this._dbContext).SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            isInsertSucc = false;
                            await transaction.RollbackAsync();
                            this._logger.LogError(ex, ex.Message);
                        }
                    }

                    //insert nella tabella dpa_ruoe_utente
                    // to do?


                    //inserimento del canale preferenziale 
                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                        {
                            corr.canalePref.systemId = (await this._dbContext.DocumentTypesEntities.AsNoTracking().
                                Where(t => t.TYPE_ID != null && t.TYPE_ID.Equals("LETTERA")).Select(t => t.SYSTEM_ID).FirstOrDefaultAsync()).ToString();
                        }


                        CanaleCorrEntity canaleCorrEntity = new()
                        {
                            ID_CORR_GLOBALE = utenteEnt.SYSTEM_ID,
                            ID_DOCUMENTTYPE = corr.canalePref.systemId.AsLong(),
                            CHA_PREFERITO = "1"
                        };

                        this._dbContext.CanaleCorrEntities.Add(canaleCorrEntity);
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }

                    if (!string.IsNullOrEmpty(utenteEnt.SYSTEM_ID.ToString()) && dataSet.Count > 0)
                    {
                        foreach (var r in dataSet)
                        {
                            string idCorrOld = r.SYSTEM_ID.ToString();
                            string idCorrNew = utenteEnt.SYSTEM_ID.ToString();
                            await this.AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }

                    corr.systemId = utenteEnt.SYSTEM_ID.ToString();

                }


                if (corr.GetType().Equals(typeof(DocsPaVO.utente.RaggruppamentoFunzionale)))
                {
                    if (!string.IsNullOrEmpty(corr.descrizione))
                    {
                        corr.oldDescrizione = corr.descrizione;
                    }

                    CorrGlobaliEntity rfInserted = await this.InsertCorrUR(corr, parent);

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(corr.codiceRubrica))
                        {
                            rfInserted.VAR_COD_RUBRICA = insertPrefix + "RF_" + rfInserted.SYSTEM_ID.ToString();
                            rfInserted.VAR_CODICE = insertPrefix + "RF_" + rfInserted.SYSTEM_ID.ToString();

                            try
                            {
                                await ((DbContext)this._dbContext).SaveChangesAsync();
                                corr.codiceRubrica = rfInserted.VAR_COD_RUBRICA;
                            }
                            catch (Exception ex)
                            {
                                isInsertSucc = false;
                                this._logger.LogError(ex, ex.Message);
                                await transaction.RollbackAsync();
                            }
                        }
                    }

                    if (((DocsPaVO.utente.RaggruppamentoFunzionale)corr).info != null)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                        this.TypedCp(((DocsPaVO.utente.RaggruppamentoFunzionale)corr).info, dettagliCorrispondente.Corrispondente.DataSet);

                        DettGlobaliEntity dettEnt = new DettGlobaliEntity()
                        {
                            ID_CORR_GLOBALI = rfInserted.SYSTEM_ID,
                            VAR_INDIRIZZO = CorrectApici(dettagliCorrispondente.Corrispondente[0].indirizzo),
                            VAR_CAP = CorrectApici(dettagliCorrispondente.Corrispondente[0].cap),
                            VAR_CITTA = CorrectApici(dettagliCorrispondente.Corrispondente[0].citta),
                            VAR_PROVINCIA = CorrectApici(dettagliCorrispondente.Corrispondente[0].provincia),
                            VAR_NAZIONE = CorrectApici(dettagliCorrispondente.Corrispondente[0].nazione),
                            VAR_TELEFONO = CorrectApici(dettagliCorrispondente.Corrispondente[0].telefono),
                            VAR_TELEFONO2 = CorrectApici(dettagliCorrispondente.Corrispondente[0].telefono2),
                            VAR_FAX = CorrectApici(dettagliCorrispondente.Corrispondente[0].fax),
                            VAR_COD_FISC = CorrectApici(dettagliCorrispondente.Corrispondente[0].codiceFiscale),
                            VAR_NOTE = CorrectApici(dettagliCorrispondente.Corrispondente[0].note),
                            VAR_LOCALITA = CorrectApici(dettagliCorrispondente.Corrispondente[0].localita),
                            VAR_LUOGO_NASCITA = CorrectApici(dettagliCorrispondente.Corrispondente[0].luogoNascita),
                            DTA_NASCITA = !string.IsNullOrEmpty(dettagliCorrispondente.Corrispondente[0].dataNascita) ? dettagliCorrispondente.Corrispondente[0].dataNascita : null,
                            VAR_TITOLO = CorrectApici(dettagliCorrispondente.Corrispondente[0].titolo),
                            VAR_COD_PI = CorrectApici(dettagliCorrispondente.Corrispondente[0].partitaIva)
                        };

                        this._dbContext.DettGlobaliEntities.Add(dettEnt);

                        try
                        {
                            await ((DbContext)this._dbContext).SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            isInsertSucc = false;
                            this._logger.LogError(ex, ex.Message);
                            await transaction.RollbackAsync();
                        }
                    }


                    //inserimento del canale preferenziale
                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                        {
                            corr.canalePref.systemId = (await this._dbContext.DocumentTypesEntities.AsNoTracking().
                                Where(t => t.TYPE_ID != null && t.TYPE_ID.Equals("LETTERA")).Select(t => t.SYSTEM_ID).FirstOrDefaultAsync()).ToString();
                        }

                        CanaleCorrEntity canaleCorrEntity = new()
                        {
                            ID_CORR_GLOBALE = rfInserted.SYSTEM_ID,
                            ID_DOCUMENTTYPE = corr.canalePref.systemId.AsLong(),
                            CHA_PREFERITO = "1"
                        };

                        this._dbContext.CanaleCorrEntities.Add(canaleCorrEntity);
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }
                    if (!string.IsNullOrEmpty(rfInserted.SYSTEM_ID.ToString()) && dataSet.Count > 0)
                    {
                        foreach (var r in dataSet)
                        {
                            string idCorrOld = r.SYSTEM_ID.ToString();
                            string idCorrNew = rfInserted.SYSTEM_ID.ToString();
                            await this.AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }
                    corr.systemId = rfInserted.SYSTEM_ID.ToString();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                isInsertSucc = false;
                await transaction.RollbackAsync();
            }
            corrToAdd = corr;
            return (isInsertSucc, corrToAdd);

        }



        private async Task<bool> AggiornaListeDistribuzione(string idCorrNew, string idCorrOld)
        {
            bool result = true;

            var listaToUp = await this._dbContext.ListeDistrEntities.Where(l => l.ID_DPA_CORR == idCorrOld.AsLong()).FirstOrDefaultAsync();

            if (listaToUp != null)
            {
                try
                {
                    listaToUp.ID_DPA_CORR = idCorrNew.AsLong();
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    result = false;
                    this._logger.LogError(exception: ex, message: ex.Message);
                }
            }

            return result;
        }

        private static Expression<Func<T, bool>> BuildAndPredicate<T>(IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            Expression<Func<T, bool>> predicate = null;

            foreach (var condition in conditions)
            {
                if (predicate == null)
                {
                    predicate = condition;
                }
                else
                {
                    var invokedExpr = Expression.Invoke(condition, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, invokedExpr), predicate.Parameters);
                }
            }

            return predicate ?? (t => false);
        }
        private async Task<CorrGlobaliEntity> InsertCorrUR(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent)
        {

            string chaTipoUrp = string.Empty;
            if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                chaTipoUrp = "U";
            else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                chaTipoUrp = "R";
            else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                chaTipoUrp = "P";
            else if (corr.GetType() == typeof(DocsPaVO.utente.RaggruppamentoFunzionale))
                chaTipoUrp = "F";



            CorrGlobaliEntity corrEntity = new()
            {
                NUM_LIVELLO = parent == null ? 0 : Int32.Parse(((DocsPaVO.utente.UnitaOrganizzativa)parent).livello) + 1,
                CHA_TIPO_IE = "E",
                ID_REGISTRO = !string.IsNullOrEmpty(corr.idRegistro) ? corr.idRegistro.AsLong() : null,
                ID_AMM = !string.IsNullOrEmpty(corr.idAmministrazione) ? corr.idAmministrazione.AsLong() : null,
                VAR_DESC_CORR = (corr.descrizione),
                ID_OLD = 0,
                DTA_INIZIO = await _dbContext.GetSystemDateTime(),
                ID_PARENT = parent == null ? 0 : parent.systemId.AsLong(),
                VAR_CODICE = !string.IsNullOrEmpty(corr.codiceCorrispondente) ? (corr.codiceCorrispondente) : (corr.codiceRubrica),
                CHA_TIPO_CORR = corr.inRubricaComune ? "C" : "S",
                CHA_TIPO_URP = chaTipoUrp,
                CHA_PA = "1",
                VAR_CODICE_AOO = corr.codiceAOO,
                VAR_COD_RUBRICA = !string.IsNullOrEmpty(corr.codiceRubrica) ? (corr.codiceRubrica) : string.Empty,
                CHA_DETTAGLI = corr.dettagli ? "1" : "0",
                VAR_EMAIL = (corr.email),
                VAR_CODICE_AMM = (corr.codiceAmm),
                VAR_CODICE_ISTAT = corr is RaggruppamentoFunzionale ? string.Empty : (((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceIstat),
                VAR_DESC_CORR_OLD = !string.IsNullOrEmpty(corr.oldDescrizione) ? (corr.oldDescrizione) : null,
                RUBRICA_ESTERNA = !string.IsNullOrEmpty(corr.rubricaEsterna) ? corr.rubricaEsterna : null,
                INTEROPURL = corr.Url != null && corr.Url.Count > 0 && !string.IsNullOrEmpty(corr.Url[0].Url) ? corr.Url[0].Url : null,
                CHA_SYSTEM_ROLE = "0"
            };

            this._dbContext.CorrGlobaliEntities.Add(corrEntity);
            await ((DbContext)this._dbContext).SaveChangesAsync();


            return corrEntity;

        }

        private async Task<bool> IsCodeCorrect(string codRubrica)
        {
            bool result = true;
            char[] separator = { ';' };
            string[] prefissi = (await this._configurationService.GetValue<string>(Resources.prefRis)).Split(separator);
            for (int i = 0; i < prefissi.Length; i++)
            {
                if (codRubrica.ToUpper().StartsWith(prefissi[i]))
                {
                    result = false;
                }
            }
            return result;
        }

        private string CheckValidity(DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente, string tipoCorr)
        {
            string messaggio = string.Empty;

            if (tipoCorr.ToUpper() == "U")
            {
                if ((dettagliCorrispondente.Corrispondente[0].codiceFiscale != null &&
                    !dettagliCorrispondente.Corrispondente[0].codiceFiscale.Equals("")) &&
                    ((dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length == 11 &&
                    this.CheckVatNmb(dettagliCorrispondente.Corrispondente[0].codiceFiscale) != 0) ||
                    (dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length == 16 &&
                    this.CheckTxCode(dettagliCorrispondente.Corrispondente[0].codiceFiscale) != 0) ||
                    (dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length != 11 && dettagliCorrispondente.Corrispondente[0].codiceFiscale.Length != 16)))
                {
                    messaggio = Resources.invalidCF;
                }
            }
            else
            {
                if (dettagliCorrispondente.Corrispondente[0].codiceFiscale != null &&
                    !dettagliCorrispondente.Corrispondente[0].codiceFiscale.Equals("") &&
                    this.CheckTxCode(dettagliCorrispondente.Corrispondente[0].codiceFiscale) != 0)
                {
                    messaggio = Resources.invalidCF;
                }
            }

            if (dettagliCorrispondente.Corrispondente[0].partitaIva != null && !dettagliCorrispondente.Corrispondente[0].partitaIva.Equals("")
                && this.CheckVatNmb(dettagliCorrispondente.Corrispondente[0].partitaIva) != 0)
            {
                messaggio = Resources.invalidVat;
            }

            return messaggio;
        }

        private void TypedCp(DataSet source, DataSet destination)
        {
            foreach (DataTable table in source.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    destination.Tables[0].ImportRow(row);
                }
            }
        }


        private int CheckVatNmb(string vatNum)
        {
            bool result = false;
            const int character = 11;
            string vatNumber = vatNum;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$");

            if (string.IsNullOrEmpty(vatNumber) || vatNum.Length != character)
                return -1;
            Match m = pregex.Match(vatNumber);
            result = m.Success;
            if (!result)
                return -2;
            result = (int.Parse(vatNumber.Substring(0, 7)) != 0);
            if (!result)
                return -3;
            result = ((int.Parse(vatNumber.Substring(7, 3)) >= 0) && (int.Parse(vatNumber.Substring(7, 3)) < 201));
            if (!result)
                return -4;

            int sum = 0;
            for (int i = 0; i < character - 1; i++)
            {
                int j = int.Parse(vatNumber.Substring(i, 1));
                if ((i + 1) % 2 == 0)
                {
                    j *= 2;
                    char[] c = j.ToString("00").ToCharArray();
                    sum += int.Parse(c[0].ToString());
                    sum += int.Parse(c[1].ToString());
                }
                else
                    sum += j;
            }
            if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!vatNumber.Substring(10, 1).Equals("0")))
                return -5;
            sum = int.Parse(vatNumber.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
            if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                return -5;
            return 0;
        }
        private int CheckTxCode(string taxCode)
        {
            taxCode = taxCode.Replace(" ", "");
            bool result = false;
            const int character = 16;
            const string omocode = "LMNPQRSTUV";
            const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            result = (string.IsNullOrEmpty(taxCode) || taxCode.Length != character);
            if (result)
                return -1;
            taxCode = taxCode.ToUpper();
            char[] arrTaxCode = taxCode.ToCharArray();
            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocode.IndexOf(arrTaxCode[k]));
                if (x != -1)
                    arrTaxCode[k] = x.ToString()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$");
            Match m = rgx.Match(new string(arrTaxCode));
            result = m.Success;
            if (!result)
                return -2;
            int somma = 0;
            arrTaxCode = taxCode.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                char c = arrTaxCode[i];
                int x = "0123456789".IndexOf(c);
                if (x != -1)
                    c = listControl.Substring(x, 1)[0];
                x = listControl.IndexOf(c);
                if ((i % 2) == 0)
                    x = listaUnequal[x];
                else
                    x = listEquivalent[x];
                somma += x;
            }
            result = (listControl.Substring(somma % 26, 1) == taxCode.Substring(15, 1));
            if (!result)
                return -3;
            return 0;
        }

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


        #endregion
    }
}
