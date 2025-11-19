// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EditingACLRequest = Pi3.App.Legacy.WebApi.Application.Requests.EditingACL;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EditingACL
{
    public class EditingACLHandler : IRequestHandler<EditingACLRequest, EditingACLResult>
    {
        #region Public Members

        public EditingACLHandler(ILogger<EditingACLHandler> logger,
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

        public async Task<EditingACLResult> Handle(EditingACLRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var description = string.Empty;
            try
            {
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var thing = request.docDiritto.idObj.AsLong();
                var accessRights = Convert.ToInt64(request.docDiritto.accessRights);
                var note = !string.IsNullOrEmpty(request.docDiritto.note) ? request.docDiritto.note : string.Format(Resources.DirittoRimossoDa, userId);

                var personOrGroup = request.docDiritto.soggetto.tipoCorrispondente.Equals("R") ? ((DocsPaVO.utente.Ruolo)request.docDiritto.soggetto).idGruppo.AsLong() 
                    : ((DocsPaVO.utente.Utente)request.docDiritto.soggetto).idPeople.AsLong();

                description = Resources.RevocaDiritto;
                var tipoDiritto = GetTipoDiritto(request.docDiritto.tipoDiritto.ToString());
                var hideDocVersions = request.docDiritto.hideDocVersions ? "1" : null;

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
                    var securityProprietarioEntity = await this._dbContext.SecurityEntities
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

                    if (securityProprietarioEntity == null || !securityProprietarioEntity.Any())
                        throw new EditingACLPi3Exception();

                    foreach (var s in securityProprietarioEntity)
                    {
                        personOrGroupRU = s.Security.PERSONORGROUP;
                        accessRight = s.Security.ACCESSRIGHTS;

                        if (s.Security.ACCESSRIGHTS == 255)
                            gruppo = s.Security.PERSONORGROUP;

                        if (idGroup != s.Security.PERSONORGROUP)
                        {
                            description += (s.ID_PEOPLE == null ? Resources.AUtente + s.USER_ID : Resources.ARuolo + s.GROUP_ID) + Resources.E;

                            var deletedSecuriryEntity = new DeletedSecurityEntity()
                            {
                                THING = thing,
                                PERSONORGROUP = s.Security.PERSONORGROUP.Value,
                                ACCESSRIGHTS = s.Security.ACCESSRIGHTS.Value,
                                ID_GRUPPO_TRASM = s.Security.ID_GRUPPO_TRASM,
                                CHA_TIPO_DIRITTO = "P",
                                NOTE = note,
                                DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                                ID_UTENTE_REV = idUser,
                                ID_RUOLO_REV = idGroup,
                                HIDE_DOC_VERSIONS = hideDocVersions
                            };

                            this._dbContext.SecurityEntities.Remove(s.Security);
                            await this._dbContext.DeletedSecurityEntities.AddAsync(deletedSecuriryEntity);
                        }
                    }

                    if(personOrGroupRU == null && accessRight != null)
                        throw new EditingACLPi3Exception();

                    //nuovo proprietario
                    //UTENTE REVOCANTE E SUO RUOLO VENGONO INSERITI COME PROPRIETARI NELLA SECURITY
                    List<SecurityEntity> securityEntitiesToInsert = new List<SecurityEntity>();

                    //UTENTE
                    securityEntitiesToInsert.Add(new SecurityEntity()
                    {
                        THING = thing,
                        PERSONORGROUP = idUser,
                        ACCESSRIGHTS = 0,
                        CHA_TIPO_DIRITTO = "P",
                        HIDE_DOC_VERSIONS = null
                    });

                    //RUOLO
                    if(idGroup != gruppo)
                    {
                        securityEntitiesToInsert.Add(new SecurityEntity()
                        {
                            THING = thing,
                            PERSONORGROUP = idGroup,
                            ACCESSRIGHTS = 255,
                            CHA_TIPO_DIRITTO = "P",
                            HIDE_DOC_VERSIONS = null
                        });
                    }

                    await this._dbContext.SecurityEntities.AddRangeAsync(securityEntitiesToInsert);
                }
                else
                {
                    //caso diritto non proprietario
                    //rimozione dalla security dell'utente a cui si è scelto di togliere i diritti

                    var securityEntity = await this._dbContext.SecurityEntities.
                        Where(s => s.THING == thing && s.ACCESSRIGHTS == accessRights && s.PERSONORGROUP == personOrGroup)
                        .Select(s => s)
                        .FirstAsync();

                    this._dbContext.SecurityEntities.Remove(securityEntity);

                    // Controllo che la tripla Thing-PersonOrGroup-Accessright non sia già inserita in DELETED_SECURITY
                    // se già presente provvedo ad aggiornare tale tripla con il diritto più alto:
                    var inserisciDeletedSecurity = true;

                    var deletedSecutityEntity = await this._dbContext.DeletedSecurityEntities
                        .FirstOrDefaultAsync(s => s.THING == thing && s.ACCESSRIGHTS == accessRights && s.PERSONORGROUP == personOrGroup);
                    if(deletedSecutityEntity != null && !string.IsNullOrEmpty(deletedSecutityEntity.CHA_TIPO_DIRITTO))
                    {
                        int priority_cha_tipo_diritto = CalcolaPrioritaChaTipoDiritto(deletedSecutityEntity.CHA_TIPO_DIRITTO);
                        int nuova_priority_cha_tipo_diritto = CalcolaPrioritaChaTipoDiritto(tipoDiritto);
                        if(priority_cha_tipo_diritto > nuova_priority_cha_tipo_diritto)
                        {
                            inserisciDeletedSecurity = false;
                        }
                        else
                        {
                            inserisciDeletedSecurity = true;
                            this._dbContext.DeletedSecurityEntities.Remove(deletedSecutityEntity);
                        }
                    }

                    if (inserisciDeletedSecurity)
                    {
                        deletedSecutityEntity = new DeletedSecurityEntity()
                        {
                            THING = thing,
                            PERSONORGROUP = personOrGroup,
                            ACCESSRIGHTS = accessRights,
                            ID_GRUPPO_TRASM = securityEntity.ID_GRUPPO_TRASM,
                            CHA_TIPO_DIRITTO = tipoDiritto,
                            NOTE = note,
                            DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                            ID_UTENTE_REV = idUser,
                            ID_RUOLO_REV = idGroup,
                            HIDE_DOC_VERSIONS = hideDocVersions,
                            CHA_COPIA_VISIBILITA = string.IsNullOrEmpty(request.docDiritto.CopiaVisibilita) ? "0" : request.docDiritto.CopiaVisibilita
                        };

                        await this._dbContext.DeletedSecurityEntities.AddAsync(deletedSecutityEntity);
                    }

                    description += request.docDiritto.soggetto.tipoCorrispondente.Equals("R") ? Resources.ARuolo : Resources.AUtente;
                    description += request.docDiritto.soggetto.codiceRubrica;
                }

                if(description.EndsWith(Resources.E))
                    description = description.Substring(0, description.Length - 3);
                description += Resources.TipoDiritto + SetTipoDiritto(request.docDiritto);

                await ((DbContext)_dbContext).SaveChangesAsync();

                var method = string.IsNullOrEmpty(request.typeObject) || request.typeObject.Equals("D") ? "EDITINGACL" : "EDITINGFASCACL";
                await this._webMethodLoggerService.LogOK(method, request.docDiritto.idObj, description);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("EDITINGACL", request.docDiritto.idObj, description);
                output = false;
            }

            return new EditingACLResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditingACLHandler> _logger;
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

        /// <summary>
        /// Calcola la priorità del CHA_TIPO_DIRITTO a parità di accessright
        /// </summary>
        /// <param name="cha_tipo_diritto"></param>
        /// <param name="priority_cha_tipo_diritto"></param>
        /// <returns></returns>
        private static int CalcolaPrioritaChaTipoDiritto(string cha_tipo_diritto)
        {
            var priority_cha_tipo_diritto = 0;

            switch (cha_tipo_diritto)
            {
                case "P":
                    priority_cha_tipo_diritto = 255;
                    break;
                case "A":
                    priority_cha_tipo_diritto = 100;
                    break;
                case "T":
                    priority_cha_tipo_diritto = 50;
                    break;
                case "F":
                    priority_cha_tipo_diritto = 25;
                    break;
            }
            return priority_cha_tipo_diritto;
        }

        private string GetTipoDiritto(string tipoDiritto)
        {
            string diritto = "";
            if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO.ToString()))
            {
                diritto = "P";
            }
            if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE.ToString()))
            {
                diritto = "T";
            }
            if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO.ToString()))
            {
                diritto = "F";
            }
            if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_SOSPESO.ToString()))
            {
                diritto = "S";
            }
            if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_ACQUISITO.ToString()))
            {
                diritto = "A";
            }
            if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO.ToString()))
            {
                diritto = "D";
            }
            return diritto;
        }


        protected string SetTipoDiritto(DocsPaVO.documento.DirittoOggetto docDir)
        {
            if (docDir.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_ACQUISITO))
                return (string.IsNullOrEmpty(docDir.CopiaVisibilita) || docDir.CopiaVisibilita.Equals("0") ? "ACQUISITO" : "ACQUISITO PER COPIA VISIBILITA");
            else
                if (docDir.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO))
                return "PROPRIETARIO";
            else
                    if (docDir.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE))
                return "TRASMISSIONE";
            else
                        if (docDir.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO))
                return "INSERIMENTO IN FASC.";
            else
                            if (docDir.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_SOSPESO))
                return "SOSPESO";
            else
                                if (docDir.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO))
                return "SOSTITUTO";
            return "";
        }
        #endregion
    }
}
