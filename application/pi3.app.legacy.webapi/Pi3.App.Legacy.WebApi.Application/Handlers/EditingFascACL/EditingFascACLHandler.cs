// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.EditingACL;
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
using EditingFascACLRequest = Pi3.App.Legacy.WebApi.Application.Requests.EditingFascACL;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EditingFascACL
{

    public class EditingFascACLHandler : IRequestHandler<EditingFascACLRequest, EditingFascACLResult>
    {
        #region Public Members

        public EditingFascACLHandler(ILogger<EditingFascACLHandler> logger,
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

        public async Task<EditingFascACLResult> Handle(EditingFascACLRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var description = string.Empty;
            long? personOrGroup = null;
            try
            {
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var thing = request.fascDiritto.idObj.AsLong();
                var rootFolder = request.fascDiritto.rootFolder.AsLong();
                var accessRights = Convert.ToInt64(request.fascDiritto.accessRights);
                var note = Resources.DirittoRimossoDa + userId;

                if(request.personOrGroup.Equals("R"))
                {
                    description = Resources.RevocaDirittiRuolo;
                    personOrGroup = ((DocsPaVO.utente.Ruolo)request.fascDiritto.soggetto).idGruppo.AsLong();
                }
                else
                {
                    description = Resources.RevocaDirittiUtente;
                    personOrGroup = ((DocsPaVO.utente.Utente)request.fascDiritto.soggetto).idPeople.AsLong();
                }
                description += request.fascDiritto.soggetto.codiceRubrica;

                var tipoDiritto = GetTipoDiritto(request.fascDiritto.tipoDiritto.ToString());

                long[] things = new long[] { thing, rootFolder };
                var securityEntity = await this._dbContext.SecurityEntities.
                       Where(s => things.Contains(s.THING.Value) && s.ACCESSRIGHTS == accessRights && s.PERSONORGROUP == personOrGroup)
                       .Select(s => s)
                       .ToListAsync();

                this._dbContext.SecurityEntities.RemoveRange(securityEntity);

                //Inserimento nella deleted_security dell'utente a cui sono stati rimossi i diritti
                List<DeletedSecurityEntity> deletedSecurityEntitiesToInsert = new List<DeletedSecurityEntity>();
                deletedSecurityEntitiesToInsert.Add(new DeletedSecurityEntity()
                {
                    THING = thing,
                    PERSONORGROUP = personOrGroup.Value,
                    ACCESSRIGHTS = accessRights,
                    ID_GRUPPO_TRASM = securityEntity[0].ID_GRUPPO_TRASM,
                    CHA_TIPO_DIRITTO = tipoDiritto,
                    NOTE = note,
                    DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                    ID_UTENTE_REV = idUser,
                    ID_RUOLO_REV = idGroup,
                    HIDE_DOC_VERSIONS = null
                });
                deletedSecurityEntitiesToInsert.Add(new DeletedSecurityEntity()
                {
                    THING = rootFolder,
                    PERSONORGROUP = personOrGroup.Value,
                    ACCESSRIGHTS = accessRights,
                    ID_GRUPPO_TRASM = securityEntity[0].ID_GRUPPO_TRASM,
                    CHA_TIPO_DIRITTO = tipoDiritto,
                    NOTE = note,
                    DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                    ID_UTENTE_REV = idUser,
                    ID_RUOLO_REV = idGroup,
                    HIDE_DOC_VERSIONS = null
                });

                await this._dbContext.DeletedSecurityEntities.AddRangeAsync(deletedSecurityEntitiesToInsert);

                //se si revocano i diritti all'utente proprietario => si revocano anche al ruolo proprietario
                //e viceversa. Il proprietario del documento diventa l'utente e il ruolo del revocante
                if (tipoDiritto.Equals("P"))
                {
                    long? personOrGroupRU = null;
                    long? accessRight = null;
                    long? gruppo = null;

                    //ex-proprietario
                    //ricerca del ruolo e utente proprietario se ruolo, 
                    //eliminazione dalla security e inserimento nella deleted_security
                    var securityProprietarioEntities = await this._dbContext.SecurityEntities
                        .GroupJoin(this._dbContext.GroupEntities, security => security.PERSONORGROUP, group => group.SYSTEM_ID, (security, group) => new { security, group })
                        .SelectMany(j => j.group.DefaultIfEmpty(), (j, group) => new { j.security, group })
                        .GroupJoin(this._dbContext.PeopleEntities, j => j.security.PERSONORGROUP, people => people.SYSTEM_ID, (j, people) => new { j.security, j.group, people })
                        .SelectMany(j => j.people.DefaultIfEmpty(), (j, people) => new { j.security, j.group, people })
                        .Where(j => j.security.THING == thing && j.security.CHA_TIPO_DIRITTO == "P")
                        .Select(j => new SecurityEditingEntity
                        {
                            Security = j.security,
                            GROUP_ID = j.group.GROUP_ID,
                            ID_GRUPPO = j.group.SYSTEM_ID,
                            USER_ID = j.people.USER_ID,
                            ID_PEOPLE = j.people.SYSTEM_ID
                        })
                        .ToListAsync();

                    if (securityProprietarioEntities == null || !securityProprietarioEntities.Any())
                        throw new EditingACLPi3Exception();

                    foreach (var s in securityProprietarioEntities)
                    {
                        personOrGroupRU = s.Security.PERSONORGROUP;
                        accessRight = s.Security.ACCESSRIGHTS;

                        description += s.ID_PEOPLE == null ? s.USER_ID : s.GROUP_ID;
                    }

                    if (personOrGroupRU == null && accessRight != null)
                        throw new EditingACLPi3Exception();

                    var securityProprietarioEntity = await this._dbContext.SecurityEntities.
                       Where(s => things.Contains(s.THING.Value) && s.ACCESSRIGHTS == accessRights && s.PERSONORGROUP == personOrGroupRU)
                       .Select(s => s)
                       .FirstOrDefaultAsync();

                    this._dbContext.SecurityEntities.RemoveRange(securityProprietarioEntity);

                    //Inserimento nella deleted_security dell'utente a cui sono stati rimossi i diritti
                    List<DeletedSecurityEntity> deletedSecurityProprietarioEntitiesToInsert = new List<DeletedSecurityEntity>();
                    deletedSecurityProprietarioEntitiesToInsert.Add(new DeletedSecurityEntity()
                    {
                        THING = thing,
                        PERSONORGROUP = personOrGroupRU.Value,
                        ACCESSRIGHTS = accessRights,
                        ID_GRUPPO_TRASM = securityEntity[0].ID_GRUPPO_TRASM,
                        CHA_TIPO_DIRITTO = "P",
                        NOTE = note,
                        DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                        ID_UTENTE_REV = idUser,
                        ID_RUOLO_REV = idGroup,
                        HIDE_DOC_VERSIONS = null
                    });
                    deletedSecurityProprietarioEntitiesToInsert.Add(new DeletedSecurityEntity()
                    {
                        THING = rootFolder,
                        PERSONORGROUP = personOrGroupRU.Value,
                        ACCESSRIGHTS = accessRights,
                        ID_GRUPPO_TRASM = securityEntity[0].ID_GRUPPO_TRASM,
                        CHA_TIPO_DIRITTO = "P",
                        NOTE = note,
                        DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                        ID_UTENTE_REV = idUser,
                        ID_RUOLO_REV = idGroup,
                        HIDE_DOC_VERSIONS = null
                    });

                    await this._dbContext.DeletedSecurityEntities.AddRangeAsync(deletedSecurityProprietarioEntitiesToInsert);
                }

                description += Resources.TipoDiritto + SetTipoDiritto(request.fascDiritto);

                await ((DbContext)_dbContext).SaveChangesAsync();

                await this._webMethodLoggerService.LogOK("EDITINGFASCACL", request.fascDiritto.idObj, description);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("EDITINGFASCACL", request.fascDiritto.idObj, description);
                output = false;
            }

            return new EditingFascACLResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditingFascACLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected class SecurityEditingEntity
        {
            public SecurityEntity Security { get; set; }
            public string? GROUP_ID { get; set; }
            public long? ID_GRUPPO { get; set; }
            public string? USER_ID { get; set; }
            public long? ID_PEOPLE { get; set; }

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

        #endregion
    }
}
