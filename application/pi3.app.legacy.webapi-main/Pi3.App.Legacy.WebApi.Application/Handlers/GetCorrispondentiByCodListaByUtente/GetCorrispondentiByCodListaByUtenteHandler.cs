// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Repositories;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate.Repositories;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCorrispondentiByCodListaByUtente
{

    // Richiede libreria MediatR
    public class GetCorrispondentiByCodListaByUtenteHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.getCorrispondentiByCodListaByUtente, getCorrispondentiByCodListaByUtenteResult>
    {
        #region Public Members

        public GetCorrispondentiByCodListaByUtenteHandler(ILogger<GetCorrispondentiByCodListaByUtenteHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext,
            IRuoloCorrispondenteRepository ruoloCorrispondenteRepository,
            IPersonaCorrispondenteRepository personaCorrispondenteRepository,
            IUOCorrispondenteRepository uoCorrispondenteRepository

            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
            this._ruoloCorrispondenteRepository = ruoloCorrispondenteRepository;
            this._personaCorrispondenteRepository = personaCorrispondenteRepository;
            this._uoCorrispondenteRepository = uoCorrispondenteRepository;
        }

        public async Task<getCorrispondentiByCodListaByUtenteResult> Handle(getCorrispondentiByCodListaByUtente request, CancellationToken cancellationToken)
        {
            var idAmm = request.idAmm.AsLong();
            ArrayList outcome = new ArrayList();

            try
            {
                var getCorList = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(_dbContext.ListeDistrEntities.AsNoTracking(), c => c.SYSTEM_ID, l => l.ID_DPA_CORR, (c, l) => new { c, l })
                    .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(), j => j.l.ID_LISTA_DPA_CORR, c2 => c2.SYSTEM_ID, (j, c2) => new { j.c, j.l, c2 }).
                             Where(w => w.c2.VAR_COD_RUBRICA.ToUpper() == request.codiceLista.ToUpper() && ((w.c2.ID_PEOPLE_LISTE == request.infoUtente.idPeople.AsLong()
                                 && w.c2.ID_GRUPPO_LISTE == null) || (w.c2.ID_PEOPLE_LISTE == null && w.c2.ID_GRUPPO_LISTE == request.infoUtente.idGruppo.AsLong())
                                 || (w.c2.ID_PEOPLE_LISTE == null && w.c2.ID_GRUPPO_LISTE == null)) && w.c.DTA_FINE == null && w.c2.ID_AMM == idAmm).
                                     OrderBy(o => o.c.VAR_DESC_CORR.Trim()).
                                         Select(s => new
                                         {
                                             idCorr = s.l.ID_DPA_CORR,
                                             idAmm = s.c2.ID_AMM,
                                             tipoCorrispondente = s.c.CHA_TIPO_IE
                                         }).ToListAsync();



                foreach (var corr in getCorList)
                {
                    outcome.Add((await _mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(
                        corr.idCorr.ToString(),
                        corr.tipoCorrispondente == "I" ? DocsPaVO.addressbook.TipoUtente.INTERNO : DocsPaVO.addressbook.TipoUtente.ESTERNO,
                        request.infoUtente)))
                        .output); 
                    //outcome.Add(await getCorrInList(corr.idCorr, idAmm));
                    //getCorrInList(ref outcome, corr.idCorr, idAmm);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new getCorrispondentiByCodListaByUtenteResult(outcome);

        }

        #endregion

        #region Private Members

        private async Task<Corrispondente> getCorrInList(long? idCorr, long idAmm)
        {
            Corrispondente outcome = new Corrispondente();

            var chaCorr = _dbContext.CorrGlobaliEntities.Where(w => w.SYSTEM_ID == idCorr).Select(s => new
            {
                s.SYSTEM_ID,
                s.ID_PEOPLE,
                s.ID_GRUPPO,
                s.CHA_TIPO_IE,
                s.CHA_TIPO_URP,
                s.CHA_TIPO_CORR

            }).FirstOrDefault();


            if (chaCorr.CHA_TIPO_IE == "I" && chaCorr.CHA_TIPO_CORR.ToUpper() != "O")
            {
                if (chaCorr.CHA_TIPO_URP == "R")
                {
                    var r = _dbContext.CorrGlobaliEntities.Where(w => w.SYSTEM_ID == idCorr).Select(s => new Ruolo()
                    {
                        systemId = s.SYSTEM_ID.ToString(),
                        codiceAOO = s.VAR_CODICE_AOO,
                        codiceAmm = s.VAR_CODICE_AMM,
                        codiceIstat = s.VAR_CODICE_ISTAT,
                        codiceRubrica = s.VAR_COD_RUBRICA,
                        descrizione = s.VAR_DESC_CORR,
                        dta_fine = s.DTA_FINE.ToString(),
                        idGruppo = s.ID_GRUPPO.ToString(),
                        tipoIE = "I",
                        tipoCorrispondente = "R"


                    }).FirstOrDefault();

                    long rIdAsLong = r.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                      .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                      .Where(x => x.b.ID_CORR_GLOBALE == rIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                      .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                      .FirstOrDefaultAsync();

                    r.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };

                    outcome = r;
                }
                else if (chaCorr.CHA_TIPO_URP == "P")
                {
                    var p = _dbContext.CorrGlobaliEntities.Where(w => w.SYSTEM_ID == idCorr).Select(s => new Utente()
                    {
                        systemId = s.SYSTEM_ID.ToString(),
                        codiceAOO = s.VAR_CODICE_AOO,
                        codiceAmm = s.VAR_CODICE_AMM,
                        idPeople = s.ID_PEOPLE.ToString(),
                        codiceRubrica = s.VAR_COD_RUBRICA,
                        descrizione = s.VAR_DESC_CORR,
                        dta_fine = s.DTA_FINE.ToString(),
                        nome = s.VAR_NOME,
                        cognome = s.VAR_COGNOME,
                        tipoIE = "I",
                        tipoCorrispondente = "P"


                    }).FirstOrDefault();

                    long pIdAsLong = p.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                      .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                      .Where(x => x.b.ID_CORR_GLOBALE == pIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                      .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                      .FirstOrDefaultAsync();

                    p.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };

                    outcome = p;
                }
                else if (chaCorr.CHA_TIPO_URP == "F")
                {
                    DocsPaVO.utente.RaggruppamentoFunzionale rf = await _dbContext.CorrGlobaliEntities.Where(w => w.SYSTEM_ID == idCorr).Select(s => new RaggruppamentoFunzionale()
                    {
                        systemId = s.SYSTEM_ID.ToString(),
                        descrizione = s.VAR_DESC_CORR,
                        codiceCorrispondente = s.VAR_CODICE,
                        codiceRubrica = s.VAR_COD_RUBRICA,
                        idRegistro = s.ID_REGISTRO.ToString() ?? string.Empty,
                        email = s.VAR_EMAIL,
                        dettagli = !string.IsNullOrEmpty(s.CHA_DETTAGLI) && s.CHA_DETTAGLI.Equals("1"),
                        idAmministrazione = s.ID_AMM.ToString(),
                        codiceAOO = s.VAR_CODICE_AOO,
                        codiceAmm = s.VAR_CODICE_AMM,
                        dta_fine = s.DTA_FINE.ToString(),
                        tipoIE = s.CHA_TIPO_IE,
                        tipoCorrispondente = "F",
                        inRubricaComune = !string.IsNullOrEmpty(s.CHA_TIPO_CORR) && s.CHA_TIPO_CORR.Equals("C"),
                        idOld = s.ID_OLD.ToString() ?? string.Empty,
                        serverPosta = new DocsPaVO.utente.ServerPosta()
                        {
                            serverSMTP = s.VAR_SMTP,
                            portaSMTP = s.NUM_PORTA_SMTP.ToString()
                        }
                    }).FirstOrDefaultAsync();

                    long rfIdAsLong = rf.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                        .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                        .Where(x => x.b.ID_CORR_GLOBALE == rfIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                        .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                        .FirstOrDefaultAsync();

                    rf.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };

                    outcome = rf;
                }
                else
                {
                    var u = _dbContext.CorrGlobaliEntities.Where(w => w.SYSTEM_ID == idCorr).Select(s => new UnitaOrganizzativa()
                    {
                        systemId = s.SYSTEM_ID.ToString(),
                        codiceAOO = s.VAR_CODICE_AOO,
                        codiceAmm = s.VAR_CODICE_AMM,
                        codiceIstat = s.VAR_CODICE_ISTAT,
                        idRegistro = s.ID_REGISTRO.ToString(),
                        codiceRubrica = s.VAR_COD_RUBRICA,
                        descrizione = s.VAR_DESC_CORR,
                        dta_fine = s.DTA_FINE.ToString(),
                        nome = s.VAR_NOME,
                        cognome = s.VAR_COGNOME,
                        tipoIE = "I",
                        tipoCorrispondente = "U"
                    }).FirstOrDefault();

                    long uIdAsLong = u.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                      .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                      .Where(x => x.b.ID_CORR_GLOBALE == uIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                      .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                      .FirstOrDefaultAsync();

                    u.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };

                    outcome = u;
                }
            }

            else if (chaCorr.CHA_TIPO_IE == "E" && chaCorr.CHA_TIPO_CORR.ToUpper() != "O")
            {
                if (chaCorr.CHA_TIPO_URP == "R")
                {
                    var rc = _ruoloCorrispondenteRepository.Get(idAmm.ToString(), chaCorr.ID_GRUPPO.ToString()).Result;
                    Ruolo r = new Ruolo()
                    {
                        canalePref = new Canale() { systemId = rc.CanalePreferenziale.Id },
                        codice = rc.Codice,
                        codiceAmm = rc.CodiceAmministrazione,
                        codiceAOO = rc.CodiceAOO,
                        rubricaEsterna = rc.RubricaEsterna,
                        inRubricaComune = (bool)rc.RubricaComune,
                        idRegistro = rc.IdRegistro,
                        systemId = rc.Id,
                        tipoIE = "E",
                        dta_fine = rc.DataFine.AsDateFormat(),
                        descrizione = rc.Description.ToString(),
                        tipoCorrispondente = "R"

                    };

                    long rIdAsLong = r.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                      .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                      .Where(x => x.b.ID_CORR_GLOBALE == rIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                      .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                      .FirstOrDefaultAsync();

                    r.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };
                    outcome = r;

                }
                else if (chaCorr.CHA_TIPO_URP == "P")
                {
                    var pc = _personaCorrispondenteRepository.Get(idAmm.ToString(), chaCorr.ID_PEOPLE.ToString()).Result;
                    Utente u = new Utente()
                    {
                        canalePref = new Canale() { systemId = pc.CanalePreferenziale.Id },
                        codiceAmm = pc.CodiceAmministrazione,
                        codfisc = pc.CodiceFiscale,
                        systemId = pc.Id,
                        tipoIE = "E",
                        codiceAOO = pc.CodiceAOO,
                        cognome = pc.Cognome,
                        nome = pc.Nome,
                        idPeople = chaCorr.ID_PEOPLE.ToString(),
                        idRegistro = pc.IdRegistro,
                        dta_fine = pc.DataFine.AsDateFormat(),
                        descrizione = pc.Description.ToString(),
                        tipoCorrispondente = "P"
                    };

                    long pIdAsLong = u.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                      .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                      .Where(x => x.b.ID_CORR_GLOBALE == pIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                      .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                      .FirstOrDefaultAsync();

                    u.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };

                    outcome = u;
                }
                else if (chaCorr.CHA_TIPO_URP == "F")
                {
                    DocsPaVO.utente.RaggruppamentoFunzionale rf = await _dbContext.CorrGlobaliEntities.Where(w => w.SYSTEM_ID == idCorr).Select(s => new RaggruppamentoFunzionale()
                    {
                        systemId = s.SYSTEM_ID.ToString(),
                        descrizione = s.VAR_DESC_CORR,
                        codiceCorrispondente = s.VAR_CODICE,
                        codiceRubrica = s.VAR_COD_RUBRICA,
                        idRegistro = s.ID_REGISTRO.ToString() ?? string.Empty,
                        email = s.VAR_EMAIL,
                        dettagli = !string.IsNullOrEmpty(s.CHA_DETTAGLI) && s.CHA_DETTAGLI.Equals("1"),
                        idAmministrazione = s.ID_AMM.ToString(),
                        codiceAOO = s.VAR_CODICE_AOO,
                        codiceAmm = s.VAR_CODICE_AMM,
                        dta_fine = s.DTA_FINE.ToString(),
                        tipoIE = s.CHA_TIPO_IE,
                        tipoCorrispondente = "F",
                        inRubricaComune = !string.IsNullOrEmpty(s.CHA_TIPO_CORR) && s.CHA_TIPO_CORR.Equals("C"),
                        idOld = s.ID_OLD.ToString() ?? string.Empty,
                        serverPosta = new DocsPaVO.utente.ServerPosta()
                        {
                            serverSMTP = s.VAR_SMTP,
                            portaSMTP = s.NUM_PORTA_SMTP.ToString()
                        }
                    }).FirstOrDefaultAsync();

                    long rfIdAsLong = rf.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                        .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                        .Where(x => x.b.ID_CORR_GLOBALE == rfIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                        .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                        .FirstOrDefaultAsync();

                    rf.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID
                    };

                    outcome = rf;
                }
                else
                {
                    var uc = _uoCorrispondenteRepository.Get(idAmm.ToString(), idCorr.ToString()).Result;
                    UnitaOrganizzativa uo = new UnitaOrganizzativa()
                    {
                        canalePref = new Canale() { systemId = uc.CanalePreferenziale.Id },
                        systemId = uc.Id,
                        tipoIE = "E",
                        codiceAmm = uc.CodiceAmministrazione,
                        codiceAOO = uc.CodiceAOO,
                        codice = uc.Codice,
                        dta_fine = uc.DataFine.AsDateFormat(),
                        idRegistro = uc.IdRegistro,
                        descrizione = uc.Description.ToString(),
                        tipoCorrispondente = "U"

                    };

                    long uoIdAsLong = uo.systemId.AsLong();

                    var canalePref = await this._dbContext.DocumentTypesEntities
                      .Join(this._dbContext.CanaleCorrEntities, a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                      .Where(x => x.b.ID_CORR_GLOBALE == uoIdAsLong && x.b.CHA_PREFERITO.Equals("1"))
                      .Select(x => new { SYSTEM_ID = x.a.SYSTEM_ID, DESCRIPTION = x.a.DESCRIPTION, TYPE_ID = x.a.TYPE_ID })
                      .FirstOrDefaultAsync();

                    uo.canalePref = new Canale()
                    {
                        systemId = canalePref.SYSTEM_ID.ToString(),
                        descrizione = canalePref.DESCRIPTION,
                        typeId = canalePref.TYPE_ID,
                        tipoCanale = canalePref.TYPE_ID
                    };

                    outcome = uo;
                }
            }
            else if (chaCorr.CHA_TIPO_CORR.ToUpper() == "O")
            {
               var o = _dbContext.CorrGlobaliEntities.Join(_dbContext.DettGlobaliEntities, c => c.SYSTEM_ID, d => d.ID_CORR_GLOBALI, (c, d) => new { c, d }).
                    Where(w => w.c.SYSTEM_ID == idCorr).Select(s => new Corrispondente()
                    {
                        indirizzo = s.d.VAR_INDIRIZZO != null ? s.d.VAR_INDIRIZZO : string.Empty,
                        citta = s.d.VAR_CITTA != null ? s.d.VAR_CITTA : string.Empty,
                        cap = s.d.VAR_CAP != null ? s.d.VAR_CAP : string.Empty,
                        prov = s.d.VAR_PROVINCIA != null ? s.d.VAR_PROVINCIA : string.Empty,
                        nazionalita = s.d.VAR_NAZIONE != null ? s.d.VAR_NAZIONE : string.Empty,
                        telefono1 = s.d.VAR_TELEFONO != null ? s.d.VAR_TELEFONO : string.Empty,
                        telefono2 = s.d.VAR_TELEFONO2 != null ? s.d.VAR_TELEFONO2 : string.Empty,
                        fax = s.d.VAR_FAX != null ? s.d.VAR_FAX : string.Empty,
                        codfisc = s.d.VAR_COD_FISC != null ? s.d.VAR_COD_FISC : string.Empty,
                        note = s.d.VAR_NOTE != null ? s.d.VAR_NOTE : string.Empty,
                        localita = s.d.VAR_LOCALITA != null ? s.d.VAR_LOCALITA : string.Empty,
                        luogoDINascita = s.d.VAR_LUOGO_NASCITA != null ? s.d.VAR_LUOGO_NASCITA : string.Empty,
                        dataNascita = s.d.DTA_NASCITA != null ? s.d.DTA_NASCITA : string.Empty,
                        titolo = s.d.VAR_TITOLO != null ? s.d.VAR_TITOLO : string.Empty,
                        systemId = s.c.SYSTEM_ID.ToString(),
                        tipoIE = "E",
                        codiceAmm = s.c.VAR_CODICE_AMM,
                        codiceAOO = s.c.VAR_CODICE_AOO,
                        dta_fine = s.c.DTA_FINE.AsDateFormat(),
                        idRegistro = s.c.ID_REGISTRO.ToString(),
                        descrizione = s.c.VAR_DESC_CORR,
                        tipoCorrispondente = "O"
                        
                    }).FirstOrDefault();

                outcome = o;
            }
            return outcome;
        }


        protected readonly ILogger<GetCorrispondentiByCodListaByUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IRuoloCorrispondenteRepository _ruoloCorrispondenteRepository;
        protected readonly IPersonaCorrispondenteRepository _personaCorrispondenteRepository;
        protected readonly IUOCorrispondenteRepository _uoCorrispondenteRepository;

        #endregion
    }

}
