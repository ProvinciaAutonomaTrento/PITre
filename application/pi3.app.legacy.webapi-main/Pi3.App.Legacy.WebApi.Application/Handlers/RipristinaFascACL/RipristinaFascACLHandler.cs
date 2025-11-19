// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.RipristinaACL;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RipristinaFascACLRequest = Pi3.App.Legacy.WebApi.Application.Requests.RipristinaFascACL;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RipristinaFascACL
{

    public class RipristinaFascACLHandler : IRequestHandler<RipristinaFascACLRequest, RipristinaFascACLResult>
    {
        #region Public Members

        public RipristinaFascACLHandler(ILogger<RipristinaFascACLHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<RipristinaFascACLResult> Handle(RipristinaFascACLRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var description = string.Empty;

            try
            {
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var thing = request.fascDiritto.idObj.AsLong();
                var rootFolder = request.fascDiritto.rootFolder.AsLong();
                var accessRights = Convert.ToInt64(request.fascDiritto.accessRights);
                long personOrGroup = 0;
                if (request.fascDiritto.soggetto.tipoCorrispondente.Equals("R"))
                {
                    description = Resources.RipristinoDirittoRuolo;
                    personOrGroup = ((DocsPaVO.utente.Ruolo)request.fascDiritto.soggetto).idGruppo.AsLong();
                }
                else
                {
                    description = Resources.RipristinoDirittoUtente;
                    personOrGroup = ((DocsPaVO.utente.Utente)request.fascDiritto.soggetto).idPeople.AsLong();
                }
                description += request.fascDiritto.soggetto.codiceRubrica;

                var tipoDiritto = GetTipoDiritto(request.fascDiritto.tipoDiritto.ToString());

                if (tipoDiritto.Equals("P"))
                {
                    //rimozione dei vecchi proprietari dalla security
                    long? personOrGroupP = null;
                    var securityProprietarioEntity = await this._dbContext.SecurityEntities
                        .Where(s => s.THING == thing && s.CHA_TIPO_DIRITTO == "P")
                        .ToListAsync();

                    if (securityProprietarioEntity == null || !securityProprietarioEntity.Any())
                        throw new RipristinaACLPi3Exception();

                    securityProprietarioEntity.ForEach(s =>
                    {
                        personOrGroupP = s.PERSONORGROUP;

                        this._dbContext.SecurityEntities.Remove(s);
                    });

                    if(personOrGroupP == null)
                        throw new RipristinaACLPi3Exception();
                }

                //ricerca di id_gruppo_trasm in deleted_security
                long[] things = new long[] {thing, rootFolder};
                var deletedSecurityEntities = await this._dbContext.DeletedSecurityEntities
                    .Where(d => things.Contains(d.THING) && d.ACCESSRIGHTS == accessRights && d.PERSONORGROUP == personOrGroup)
                    .ToListAsync();

                //rimozione dalla deleted_security dell'utente a cui ripristinare i diritti
                this._dbContext.DeletedSecurityEntities.RemoveRange(deletedSecurityEntities);

                //inserimento nella security dell'utente a cui devono essere ripristinati i diritti
                List<SecurityEntity> securityEntitiesToInsert = new List<SecurityEntity>();
                securityEntitiesToInsert.Add(new SecurityEntity()
                {
                    THING = thing,
                    PERSONORGROUP = personOrGroup,
                    ACCESSRIGHTS = accessRights,
                    ID_GRUPPO_TRASM = deletedSecurityEntities[0].ID_GRUPPO_TRASM,
                    CHA_TIPO_DIRITTO = tipoDiritto,
                    HIDE_DOC_VERSIONS = null
                });
                securityEntitiesToInsert.Add(new SecurityEntity()
                {
                    THING = rootFolder,
                    PERSONORGROUP = personOrGroup,
                    ACCESSRIGHTS = accessRights,
                    ID_GRUPPO_TRASM = deletedSecurityEntities[0].ID_GRUPPO_TRASM,
                    CHA_TIPO_DIRITTO = tipoDiritto,
                    HIDE_DOC_VERSIONS = null
                });

                await this._dbContext.SecurityEntities.AddRangeAsync(securityEntitiesToInsert);

                if(tipoDiritto == "P")
                {
                    //se si ripristinano i diritti all'utente proprietario => si ripristinano anche al ruolo proprietario e viceversa.
                    long? personOrGroupRU = null;
                    long? accessRight = null;
                    var codiceRubrica = string.Empty;
                    long? idGruppoTrasmRU = null;

                    //ex-proprietario
                    //ricerca del ruolo se utente o ricerca dell'utente proprietario se ruolo
                    //il record trovato va eliminato dalla deleted_security e aggiunto nella security
                    var deletedSecurityProprietarioEntities = await this._dbContext.DeletedSecurityEntities
                      .GroupJoin(this._dbContext.GroupEntities, security => security.PERSONORGROUP, group => group.SYSTEM_ID, (security, group) => new { security, group })
                      .SelectMany(j => j.group.DefaultIfEmpty(), (j, group) => new { j.security, group })
                      .GroupJoin(this._dbContext.PeopleEntities, j => j.security.PERSONORGROUP, people => people.SYSTEM_ID, (j, people) => new { j.security, j.group, people })
                      .SelectMany(j => j.people.DefaultIfEmpty(), (j, people) => new { j.security, j.group, people })
                      .Where(j => j.security.THING == thing && j.security.CHA_TIPO_DIRITTO == "P")
                      .Select(j => new DeletedSecurityRipristinaACLEntity
                      {
                          DeletedSecurity = j.security,
                          GROUP_ID = j.group.GROUP_ID,
                          ID_GRUPPO = j.group.SYSTEM_ID,
                          USER_ID = j.people.USER_ID,
                          ID_PEOPLE = j.people.SYSTEM_ID
                      })
                      .ToListAsync();

                    if(deletedSecurityProprietarioEntities == null || !deletedSecurityProprietarioEntities.Any())
                        throw new RipristinaACLPi3Exception();

 
                    deletedSecurityProprietarioEntities.ForEach(d =>
                    {
                        personOrGroupRU = d.DeletedSecurity.PERSONORGROUP;
                        accessRight = d.DeletedSecurity.ACCESSRIGHTS;
                        codiceRubrica = d.USER_ID == null ? d.GROUP_ID : d.USER_ID;
                        idGruppoTrasmRU = d.DeletedSecurity.ID_GRUPPO_TRASM;

                        //rimozione dalla deleted_security 
                        this._dbContext.DeletedSecurityEntities.Remove(d.DeletedSecurity);
                    });

                    if (personOrGroupRU == null || accessRight != null)
                        throw new RipristinaFascACLPi3Exception();

                    description += request.fascDiritto.soggetto.tipoCorrispondente.Equals("R") ? Resources.EAUtente : Resources.EARuolo;
                    description += codiceRubrica;

                    //inserimento nella security
                    List<SecurityEntity> securityEntitiesProprietarioToInsert = new List<SecurityEntity>();
                    securityEntitiesToInsert.Add(new SecurityEntity()
                    {
                        THING = thing,
                        PERSONORGROUP = personOrGroupRU,
                        ACCESSRIGHTS = accessRight,
                        ID_GRUPPO_TRASM = idGruppoTrasmRU,
                        CHA_TIPO_DIRITTO = "P",
                        HIDE_DOC_VERSIONS = null
                    });
                    securityEntitiesToInsert.Add(new SecurityEntity()
                    {
                        THING = rootFolder,
                        PERSONORGROUP = personOrGroupRU,
                        ACCESSRIGHTS = accessRight,
                        ID_GRUPPO_TRASM = idGruppoTrasmRU,
                        CHA_TIPO_DIRITTO = "P",
                        HIDE_DOC_VERSIONS = null
                    });
                }

                description += Resources.TipoDiritto + SetTipoDiritto(request.fascDiritto);


                await ((DbContext)_dbContext).SaveChangesAsync();

                await this._webMethodLoggerService.LogOK("EDITINGFASCACL", request.fascDiritto.idObj, description);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("EDITINGACL", request.fascDiritto.idObj, description);
                output = false;
            }

            return new RipristinaFascACLResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RipristinaFascACLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected class DeletedSecurityRipristinaACLEntity
        {
            public DeletedSecurityEntity DeletedSecurity { get; set; }
            public string? GROUP_ID { get; set; }
            public long? ID_GRUPPO { get; set; }
            public string? USER_ID { get; set; }
            public long? ID_PEOPLE { get; set; }

        }

        protected string SetTipoDiritto(DocsPaVO.fascicolazione.DirittoOggetto fascDir)
        {
            if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO))
                return (string.IsNullOrEmpty(fascDir.CopiaVisibilita) || fascDir.CopiaVisibilita.Equals("0") ? "ACQUISITO" : "ACQUISITO PER COPIA VISIBILITA");
            else
                if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO))
                return "PROPRIETARIO";
            else
                    if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE))
                return "TRASMISSIONE";
            else
                        if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO))
                return "SOSPESO";
            else
                            if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                return "SOSTITUTO";
            return "";
        }

        protected string GetTipoDiritto(string tipoDiritto)
        {
            string diritto = "";
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO.ToString()))
            {
                diritto = "P";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE.ToString()))
            {
                diritto = "T";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO.ToString()))
            {
                diritto = "S";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO.ToString()))
            {
                diritto = "A";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO.ToString()))
            {
                diritto = "D";
            }
            return diritto;
        }

        #endregion
    }

}
