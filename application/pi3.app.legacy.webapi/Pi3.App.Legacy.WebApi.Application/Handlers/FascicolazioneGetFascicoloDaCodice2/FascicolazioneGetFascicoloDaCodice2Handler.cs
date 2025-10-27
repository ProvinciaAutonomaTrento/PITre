// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneGetFascicoloDaCodice2Request = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetFascicoloDaCodice2;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFascicoloDaCodice2
{
    public class FascicolazioneGetFascicoloDaCodice2Handler : IRequestHandler<FascicolazioneGetFascicoloDaCodice2Request, FascicolazioneGetFascicoloDaCodice2Result>
    {
        #region Public Members

        public FascicolazioneGetFascicoloDaCodice2Handler(
            ILogger<FascicolazioneGetFascicoloDaCodice2Handler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneGetFascicoloDaCodice2Result> Handle(FascicolazioneGetFascicoloDaCodice2Request request, CancellationToken cancellationToken)
        {
            DocsPaVO.fascicolazione.Fascicolo output= new();
            try
            {
                output = await this.GetFascDaCodice2(request.idAmministrazione,request.idGruppo,request.idPeople,request.registro,request.enableUffRef,request.enableProfilazione,request.codiceFascicolo,request.idTitolario);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFascicoloDaCodice2Handler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        private async Task<DocsPaVO.fascicolazione.Fascicolo> GetFascDaCodice2(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, string codFascicolo, string idTitolario)
        {
            var res = new DocsPaVO.fascicolazione.Fascicolo();

            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
            long idGroupAsLong = idGroup.AsLong();
            long idUserAsLong = idUser.AsLong();


            var predicate = PredicateBuilder.New<ProjWithTemp>();

            IQueryable<ProjWithTemp> projectEntities = this._dbContext.ProjectEntities.AsNoTracking()
                        .Where(x => (x.ID_AMM == null || x.ID_AMM == idAmministrazione.AsLong()) &&
                        this._dbContext.SecurityEntities.Any(s => x.SYSTEM_ID == s.THING &&
                            (s.PERSONORGROUP == idUserAsLong || s.PERSONORGROUP == idGroupAsLong) &&
                            s.ACCESSRIGHTS > 0) && x.CHA_TIPO_PROJ != null && x.CHA_TIPO_PROJ.Equals("F")).Select(p => new ProjWithTemp()
                            {
                                A = p,
                                F = null
                            });

            if (!(idTitolario == null || idTitolario == ""))
            {
                List<string> idTitolari = (idTitolario.Split(',')).ToList();
                predicate = predicate.And( p => p.A.ID_TITOLARIO != null && idTitolari.Contains(p.A.ID_TITOLARIO.ToString()));
            }

            if (registro != null)
                predicate = predicate.And( p => (p.A.ID_REGISTRO == null || p.A.ID_REGISTRO == registro.systemId.AsLong()));



            if (codFascicolo != null && codFascicolo != "")
                predicate = predicate.And( p => p.A.VAR_CODICE != null && p.A.VAR_CODICE.ToUpper().Equals(codFascicolo));


            projectEntities = projectEntities.Where(predicate);
            if (enableProfilazione)
            {
                projectEntities = (from a in projectEntities
                                    join f in this._dbContext.AssTemplatesFascEntities.AsNoTracking() on a.A.SYSTEM_ID.ToString() equals f.ID_PROJECT into fj
                                    from fi in fj.DefaultIfEmpty()
                                    select new ProjWithTemp()
                                    {
                                    A = a.A,
                                    F = fi
                                    });
            }
            var proj = await projectEntities.Select(
                a => new
                {
                    a.A.SYSTEM_ID,
                    a.A.DESCRIPTION,
                    a.A.CHA_TIPO_PROJ,
                    a.A.VAR_CODICE,
                    a.A.ID_AMM,
                    a.A.NUM_LIVELLO,
                    a.A.CHA_TIPO_FASCICOLO,
                    a.A.ID_FASCICOLO,
                    a.A.ID_PARENT,
                    a.A.VAR_COD_ULTIMO,
                    a.A.VAR_NOTE,
                    DTA_APERTURA = a.A.DTA_APERTURA,
                    DTA_CHIUSURA = a.A.DTA_CHIUSURA,
                    a.A.CHA_STATO,
                    a.A.ID_TIPO_PROC,
                    a.A.ID_REGISTRO,
                    CODREG = IPi3DbContextMappedFunctions.GetCodReg(a.A.ID_REGISTRO.GetValueOrDefault()),
                    a.A.ID_UO_LF,
                    a.A.DTA_UO_LF,
                    a.A.DTA_CREAZIONE,
                    a.A.ID_UO_REF,
                    a.A.CARTACEO,
                    a.A.CHA_PRIVATO,
                    a.A.CHA_PUBBLICO,
                    a.A.ID_TITOLARIO,
                    DTA_SCADENZA = a.A.DTA_SCADENZA.AsDateFormat(),
                    a.A.NUM_MESI_CONSERVAZIONE,
                    a.A.NUM_FASCICOLO,
                    a.A.AUTHOR,
                    a.A.ID_RUOLO_CREATORE,
                    a.A.ID_UO_CREATORE,
                    a.A.ID_PEOPLE_DELEGATO,
                    CHA_CONSENTI_CLASS = IPi3DbContextMappedFunctions.GetChaConsentiClass(a.A.ID_PARENT.GetValueOrDefault(), a.A.CHA_TIPO_PROJ, a.A.ID_FASCICOLO.GetValueOrDefault()) ,
                    CHA_CONSENTI_FASC = IPi3DbContextMappedFunctions.GetChaConsentiFasc(a.A.ID_PARENT.GetValueOrDefault(), a.A.CHA_TIPO_PROJ, a.A.CHA_TIPO_FASCICOLO, a.A.ID_FASCICOLO.GetValueOrDefault()) ,
                    in_adl = IPi3DbContextMappedFunctions.GetInAdl(a.A.SYSTEM_ID, "F", idGruppo.AsLong(), idPeople.AsLong()) ,
                    a.A.ID_PIANO_CONSERVAZIONE,
                    //F = a.F
                }
                ).FirstOrDefaultAsync();

            
            if (proj != null)
            {
                var rights = await _dbContext.GetSecurityRights(proj.SYSTEM_ID.ToString(), idUser, idGroup);
                var rigthsAsLong = Convert.ToInt32(rights);

                var creatoreFasc = new DocsPaVO.fascicolazione.CreatoreFascicolo()
                {
                    idPeople = proj.AUTHOR != null ? proj.AUTHOR.ToString() : string.Empty,
                    idCorrGlob_Ruolo = proj.ID_RUOLO_CREATORE != null ? proj.ID_RUOLO_CREATORE.ToString() : string.Empty,
                    idCorrGlob_UO = proj.ID_UO_CREATORE != null ? proj.ID_UO_CREATORE.ToString() : string.Empty,
                    idPeopleDelegato = proj.ID_PEOPLE_DELEGATO 
                };

                if (string.Compare(creatoreFasc.idCorrGlob_UO.Trim(), string.Empty, true) != 0)
                {
                    creatoreFasc.uo_codiceCorrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(u => u.SYSTEM_ID == creatoreFasc.idCorrGlob_UO.AsLong()).Select(u => u.VAR_CODICE).FirstOrDefaultAsync(); 
                }


                res = new DocsPaVO.fascicolazione.Fascicolo()
                {
                    systemID = proj.SYSTEM_ID.ToString(),
                    apertura = proj.DTA_APERTURA?.ToString().Trim(),
                    chiusura = proj.DTA_CHIUSURA?.ToString().Trim(),
                    codice = proj.VAR_CODICE?.ToString(),
                    descrizione = proj.DESCRIPTION?.ToString(),
                    stato = proj.CHA_STATO?.ToString(),
                    tipo = proj.CHA_TIPO_FASCICOLO?.ToString(),
                    idClassificazione = proj.ID_PARENT?.ToString(),
                    codUltimo = proj.VAR_COD_ULTIMO?.ToString(),
                    idRegistroNodoTit = proj.ID_REGISTRO?.ToString(),
                    numMesiConservazione = proj.NUM_MESI_CONSERVAZIONE?.ToString(),
                    pubblico = !string.IsNullOrEmpty(proj.CHA_PUBBLICO) ? proj.CHA_PUBBLICO.Equals("1") : false,
                    idTitolario = proj.ID_TITOLARIO.ToString(),
                    accessRights = GetAccessRigths(rights),
                    //codiceRegistroNodoTit = f.CODREG,
                    dtaLF = proj.DTA_UO_LF?.ToString(),
                    idUoLF = proj.ID_UO_LF?.ToString(),
                    cartaceo = Convert.ToInt32(proj.CARTACEO) > 0,
                    privato = proj.CHA_PRIVATO,
                    dtaScadenza = proj.DTA_SCADENZA,
                    numFascicolo = proj.NUM_FASCICOLO?.ToString(),
                    codiceRegistroNodoTit = await GetCodReg(proj.ID_REGISTRO),
                    isFascicolazioneConsentita = proj.CHA_CONSENTI_FASC != "0",
                    creatoreFascicolo = creatoreFasc,
                    template = (await (this._mediator.Send(new Application.Requests.getTemplateFascDettagli(proj.SYSTEM_ID.ToString())))).output

                };

                if (enableUfficioRef)
                {
                    if ((res.tipo == null || !res.tipo.Equals("G")) && proj.ID_UO_REF != null)
                    {
                        DocsPaVO.utente.Corrispondente corrUr = new DocsPaVO.utente.Corrispondente();
                        corrUr.systemId = proj.ID_UO_REF != null ? proj.ID_UO_REF.ToString() : string.Empty;
                        res.ufficioReferente = corrUr;
                    }
                    else
                    {
                        res.ufficioReferente = null;
                    }
                }
                    


            }
            
            return res;
        }
        private async Task<string> GetCodReg(long? idRegistro)
        {
            return await this._dbContext.RegistroEntities.Where(x => x.SYSTEM_ID == idRegistro).Select(x => x.VAR_CODICE).FirstOrDefaultAsync();
        }
        private string GetAccessRigths(SecurityRightTypesEnum rights)
        {
            switch (rights)
            {
                case SecurityRightTypesEnum.FullControl:
                    return "255";
                    break;
                case SecurityRightTypesEnum.Write:
                    return "63";
                    break;
                case SecurityRightTypesEnum.Read:
                    return "45";
                    break;
                default:
                    return "-1";
                    break;
            }
        }

        private class ProjWithTemp
        {
            public ProjectEntity? A{ get; set; }
            public AssTemplatesFascEntity? F{ get; set; }
        }

        #endregion
    }
}