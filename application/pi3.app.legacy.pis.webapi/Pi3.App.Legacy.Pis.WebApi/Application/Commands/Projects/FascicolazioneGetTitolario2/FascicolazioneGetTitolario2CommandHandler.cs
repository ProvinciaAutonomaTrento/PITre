// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetTitolario2
{
    public class FascicolazioneGetTitolario2CommandHandler : IRequestHandler<FascicolazioneGetTitolario2Command, FascicolazioneGetTitolario2CommandResponse>
    {
        public FascicolazioneGetTitolario2CommandHandler(ILogger<FascicolazioneGetTitolario2CommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<FascicolazioneGetTitolario2CommandResponse> Handle(FascicolazioneGetTitolario2Command request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.fascicolazione.Classificazione> output = new ();

            try
            {
                string idRegistro = "";
                string varCodLiv1 = "";
                string separatore = GetSeparatore((_claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant)).AsLong());
                char[] separator = { separatore[0] };

                string IdTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);


                var queryTitolario = (from p in _dbContext.ProjectEntities
                                      where p.ID_AMM.Equals(IdTenant.AsLong()) && p.CHA_TIPO_PROJ == "T"
                                      select new Classificazioni
                                      {
                                          SYSTEM_ID = p.SYSTEM_ID,
                                          DESCRIPTION = p.DESCRIPTION,
                                          ID_PARENT = p.ID_PARENT,
                                          VAR_CODICE = p.VAR_CODICE,
                                          NUM_LIVELLO = p.NUM_LIVELLO,
                                          VAR_COD_ULTIMO = p.VAR_COD_ULTIMO,
                                          VAR_COD_LIV1 = p.VAR_COD_LIV1,
                                          ID_TIPO_FASC = p.ID_TIPO_FASC,
                                          CHA_BLOCCA_FASC = p.CHA_BLOCCA_FASC,
                                          ID_TITOLARIO = p.ID_TITOLARIO,
                                          CHA_STATO = p.CHA_STATO,
                                          DTA_ATTIVAZIONE = p.DTA_ATTIVAZIONE,
                                          DTA_CESSAZIONE = p.DTA_CESSAZIONE,
                                          VAR_NOTE = p.VAR_NOTE,
                                          ID_REGISTRO = p.ID_REGISTRO
                                      });

                if (!string.IsNullOrEmpty(request.IdGruppo))
                {
                    queryTitolario = queryTitolario.Join(this._dbContext.SecurityEntities.AsNoTracking(), prj => prj.SYSTEM_ID, sec => sec.THING, (prj, sec) => new
                    {
                        prj = prj,
                        sec.ACCESSRIGHTS,
                        sec.PERSONORGROUP
                    })
                        .Where(p => p.ACCESSRIGHTS > 0 && (p.PERSONORGROUP == request.IdGruppo.AsLong() || p.PERSONORGROUP == request.IdPeople.AsLong()))
                        .Select(p => p.prj);
                }

                if (request.Registro != null)
                {
                    idRegistro = request.Registro.systemId;
                    queryTitolario = queryTitolario.Where(qt => qt.ID_REGISTRO == null || qt.ID_REGISTRO == idRegistro.AsLong());
                }
                bool estraiTitolario = true;

                if (!string.IsNullOrEmpty(request.CodiceClassifica))
                {

                    varCodLiv1 = await getCodLiv1_2(idRegistro, request.IdTitolario, IdTenant, request.CodiceClassifica);

                    if (!String.IsNullOrEmpty(varCodLiv1))
                    {
                        if (request.GetFigli)
                        {
                            queryTitolario = queryTitolario.Where(qt => qt.VAR_COD_LIV1.StartsWith(varCodLiv1));
                        }
                        else
                        {
                            queryTitolario = queryTitolario.Where(qt => qt.VAR_COD_LIV1.Equals(varCodLiv1));
                        }
                    }
                    else
                    {
                        estraiTitolario = false;
                    }
                }
                if (!String.IsNullOrEmpty(request.IdTitolario))
                {
                    var inTitolario = request.IdTitolario.Contains(",") ? request.IdTitolario.Split(",") : new string[] { request.IdTitolario };
                    queryTitolario = queryTitolario.Where(qt => inTitolario.Contains(qt.ID_TITOLARIO.ToString()));
                }

                queryTitolario = queryTitolario.OrderBy(qt => qt.NUM_LIVELLO).ThenBy(qt => qt.VAR_COD_LIV1);

                var classificazioni = await queryTitolario.ToListAsync();

                if (estraiTitolario)
                {
                    string numLivello = (classificazioni[0].NUM_LIVELLO).ToString();

                    foreach (var classTit in classificazioni.Where(c => c.NUM_LIVELLO == numLivello.AsLong()))
                    {
                        Classificazione rootClass = new Classificazione();
                        rootClass.systemID = classTit.SYSTEM_ID.ToString();
                        rootClass.descrizione = classTit.DESCRIPTION;
                        rootClass.codice = classTit.VAR_CODICE;
                        rootClass.varcodliv1 = classTit.VAR_COD_LIV1;
                        rootClass.codUltimo = !String.IsNullOrEmpty(classTit.VAR_COD_ULTIMO) ? (Int32.Parse(classTit.VAR_COD_ULTIMO) + 1).ToString() : "1";
                        //Introdotto per tenere traccia dell'id registro del nodo di titolario
                        rootClass.idRegistroNodoTit = classTit.ID_REGISTRO.Value.ToString();
                        rootClass.idTipoFascicolo = (classTit.ID_TIPO_FASC != null) ? classTit.ID_TIPO_FASC.Value.ToString() : "";
                        rootClass.bloccaTipoFascicolo = classTit.CHA_BLOCCA_FASC;

                        rootClass.codiceRegistroNodoTit = !String.IsNullOrEmpty(rootClass.idRegistroNodoTit) ? await this._dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == rootClass.idRegistroNodoTit.AsLong()).Select(r => r.VAR_CODICE).FirstOrDefaultAsync() : "";
                        if (request.Registro != null)
                        {
                            rootClass.registro = request.Registro;
                        }
                        else if (classTit.ID_REGISTRO != null)
                        {
                            rootClass.registro = GetRegistro(classTit.ID_REGISTRO);
                        }

                        rootClass.childs = this.GetClassificazioni(rootClass.codice, classificazioni, rootClass.systemID, request.Registro, separatore);

                        output.Add(rootClass);
                    }
                }


            }
            catch (Exception ex)
            {

                _logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new()
            {
                Output = output != null ? output.ToArray() : new Classificazione[0]
            };
        }


        #region Private Members

        protected readonly ILogger<FascicolazioneGetTitolario2CommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IPi3DbContext _dbContext;

        protected virtual async Task<string> getCodLiv1_2(string idRegistro, string idTitolario, string idTenant, string codiceClassifica)
        {
            var query = (from p in _dbContext.ProjectEntities
                         where p.ID_AMM.Equals(idTenant.AsLong()) && p.VAR_CODICE.ToUpper().Equals(codiceClassifica.ToUpper()) && p.CHA_TIPO_PROJ.Equals("T")
                         select new
                         {
                             SYSTEM_ID = p.SYSTEM_ID,
                             VAR_COD_LIV1 = p.VAR_COD_LIV1,
                             ID_TITOLARIO = p.ID_TITOLARIO,
                             ID_REGISTRO = p.ID_REGISTRO
                         });

            if (!String.IsNullOrEmpty(idRegistro))
            {
                query = query.Where(p => p.ID_REGISTRO == null || p.ID_REGISTRO == idRegistro.AsLong());
            }

            if (!String.IsNullOrEmpty(idTitolario))
            {
                var inTitolario = idTitolario.Contains(",") ? idTitolario.Split(",") : new string[] { idTitolario };
                query = query.Where(p => p.ID_TITOLARIO == null || inTitolario.Contains(p.ID_TITOLARIO.ToString()));
            }

            var varCodLiv1 = await query.FirstOrDefaultAsync();

            return varCodLiv1 != null ? varCodLiv1.VAR_COD_LIV1 : null;

        }


        protected class Classificazioni
        {
            public long SYSTEM_ID { get; set; }
            public string? DESCRIPTION { get; set; }
            public long? ID_PARENT { get; set; }
            public string? VAR_CODICE { get; set; }
            public long? NUM_LIVELLO { get; set; }
            public string? VAR_COD_ULTIMO { get; set; }
            public long? ID_REGISTRO { get; set; }
            public string VAR_COD_LIV1 { get; set; }
            public long? ID_TIPO_FASC { get; set; }
            public string? CHA_BLOCCA_FASC { get; set; }
            public long? ID_TITOLARIO { get; set; }
            public string? CHA_STATO { get; set; }
            public DateTime? DTA_ATTIVAZIONE { get; set; }
            public DateTime? DTA_CESSAZIONE { get; set; }
            public string? VAR_NOTE { get; set; }
        }

        private Registro GetRegistro(long? idRegistro)
        {
            Registro r = null;

            if (!idRegistro.HasValue && idRegistro == 0)
                return r;



            r = _dbContext.RegistroEntities.Where(w => w.SYSTEM_ID == idRegistro).Select(s => new Registro()
            {
                systemId = s.SYSTEM_ID.ToString(),
                codRegistro = s.VAR_CODICE,
                codice = s.NUM_RIF.ToString(),
                descrizione = s.VAR_DESC_REGISTRO,
                email = s.VAR_EMAIL_REGISTRO,
                stato = s.CHA_STATO,
                idAmministrazione = s.ID_AMM.ToString(),
                dataApertura = s.DTA_OPEN.AsDateFormat(),
                dataChiusura = s.DTA_CLOSE.AsDateFormat(),
                dataUltimoProtocollo = s.DTA_ULTIMO_PROTO.AsDateFormat(),
                idRuoloAOO = s.ID_RUOLO_AOO.ToString(),
                idRuoloResp = s.ID_RUOLO_RESP.ToString(),
                idUtenteAOO = s.ID_PEOPLE_AOO.ToString(),
                autoInterop = s.CHA_AUTO_INTEROP,
                chaRF = s.CHA_RF,
                rfDisabled = s.CHA_DISABILITATO,
                Sospeso = !string.IsNullOrEmpty(s.CHA_DISABILITATO) ? s.CHA_DISABILITATO.Equals("1") ? true : false : false,
                idAOOCollegata = s.ID_AOO_COLLEGATA.ToString(),
                invioRicevutaManuale = s.INVIO_RICEVUTA_MANUALE.ToString(),
                FlagWspia = "0",
                flag_pregresso = !string.IsNullOrEmpty(s.VAR_PREG) ? s.VAR_PREG.Equals("1") ? true : false : false,
                anno_pregresso = s.ANNO_PREG,
                Diritto_Ruolo_AOO = s.DIRITTO_RUOLO_AOO.ToString(),
                codiceIpa = s.VAR_CODICE_IPA

            }).FirstOrDefault();

            long idAmm = long.Parse(r.idAmministrazione);

            if (string.IsNullOrEmpty(r.codAmministrazione))
                r.codAmministrazione = _dbContext.AmministraEntities.Where(w => w.SYSTEM_ID == idAmm).Select(s => s.VAR_DESC_AMM).First();

            return r;
        }


        private object[] GetClassificazioni(string codice_parent, List<Classificazioni> classificazioni, string parent_id, DocsPaVO.utente.Registro registro, string separator)
        {
            /* Non inserire output su file di log o su debug perche' la procedura e' 
             * ricorsiva ed ogni informazione scritta rende meno leggibile il log.
             */
            List<Classificazioni> childs = classificazioni.Where(c => c.ID_PARENT == parent_id.AsLong()).ToList(); //table.Select("ID_PARENT=" + parent_id);
            System.Collections.ArrayList classificazioniOutput = new System.Collections.ArrayList();

            foreach (Classificazioni dr in childs)
            {
                DocsPaVO.fascicolazione.Classificazione classificazione = new DocsPaVO.fascicolazione.Classificazione();
                classificazione.codice = dr.VAR_CODICE.ToString();
                classificazione.descrizione = dr.DESCRIPTION.ToString();
                classificazione.systemID = dr.SYSTEM_ID.ToString();
                classificazione.codUltimo = !String.IsNullOrEmpty(dr.VAR_COD_ULTIMO) ? (Int32.Parse(dr.VAR_COD_ULTIMO) + 1).ToString() : "1";

                if (registro != null)
                {
                    classificazione.registro = registro;
                }
                else if (dr.ID_REGISTRO != null)
                {
                    classificazione.registro = GetRegistro(dr.ID_REGISTRO);
                }

                object[] classChildren = GetClassificazioni(classificazione.codice, classificazioni, dr.SYSTEM_ID.ToString(), registro, separator);

                for (int j = 0; j < classChildren.Length; j++)
                {
                    classificazione.childs[j] = classChildren[j];
                }

                classificazioniOutput.Add(classificazione);
            }

            return classificazioniOutput.ToArray();
        }


        private string GetSeparatore(long idAmm)
        {
            string separatore = _dbContext.AmministraEntities.Where(w => w.SYSTEM_ID == idAmm).Select(s => s.CHA_SEPARATORE).FirstOrDefault();

            if (string.IsNullOrEmpty(separatore))
                separatore = "/";

            return separatore;
        }
        #endregion

    }
}
