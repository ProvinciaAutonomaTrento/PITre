// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using RipristinaACLRequest = Pi3.App.Legacy.WebApi.Application.Requests.RipristinaACL;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RipristinaACL
{
    public class RipristinaACLHandler : IRequestHandler<RipristinaACLRequest, RipristinaACLResult>
    {
        #region Public Members

        public RipristinaACLHandler(ILogger<RipristinaACLHandler> logger,
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

        public async Task<RipristinaACLResult> Handle(RipristinaACLRequest request, CancellationToken cancellationToken)
        {

            var output = true;
            var description = Resources.RipristinoDiritto;

            try
            {
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var thing = request.docDiritto.idObj.AsLong();
                var accessRights = Convert.ToInt64(request.docDiritto.accessRights);
                var personOrGroup = request.docDiritto.soggetto.tipoCorrispondente.Equals("R") ? 
                    ((DocsPaVO.utente.Ruolo)request.docDiritto.soggetto).idGruppo.AsLong()
                   : ((DocsPaVO.utente.Utente)request.docDiritto.soggetto).idPeople.AsLong();
                var tipoDiritto = GetTipoDiritto(request.docDiritto.tipoDiritto.ToString());
                var hideDocVersions = request.docDiritto.hideDocVersions ? "1" : null;

                if (tipoDiritto.Equals("P"))
                {
                    long? personOrGroupP = null;

                    //caso in cui si vuole ripristinare diritto di proprietà
                    //Verifica se ruolo proprietario del revocato è diverso dal revocante
                    var countDeletedSecurityEntities = await this._dbContext.DeletedSecurityEntities
                        .CountAsync(d => !this._dbContext.SecurityEntities.Any(s => s.THING == d.THING && s.ACCESSRIGHTS > 20 && s.PERSONORGROUP == d.PERSONORGROUP)
                                    && d.THING == thing && d.CHA_TIPO_DIRITTO == tipoDiritto);

                    //1.rimozione dei vecchi proprietari dalla security
                    var securityProprietarioEntities = await this._dbContext.SecurityEntities
                        .Where(s => s.THING == thing && s.CHA_TIPO_DIRITTO == "P")
                        .ToListAsync();

                    if (securityProprietarioEntities == null || !securityProprietarioEntities.Any())
                        throw new RipristinaACLPi3Exception();

                    securityProprietarioEntities.ForEach(s =>
                    {
                        personOrGroupP = s.PERSONORGROUP;
                        if(idGroup != personOrGroupP || countDeletedSecurityEntities == 2)
                        {
                            this._dbContext.SecurityEntities.Remove(s);
                        }
                    });

                    if(personOrGroupP == null)
                        throw new RipristinaACLPi3Exception();

                    //2.rimozione dalla deleted_security del nuovo proprietario 
                    //e inserimento nella security
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

                    if (deletedSecurityProprietarioEntities == null || !deletedSecurityProprietarioEntities.Any())
                        throw new RipristinaACLPi3Exception();

                    long? personOrGroupRU = null;
                    long? accessRight = null;

                    foreach (var s in deletedSecurityProprietarioEntities)
                    {
                        personOrGroupRU = s.DeletedSecurity.PERSONORGROUP;
                        accessRight = s.DeletedSecurity.ACCESSRIGHTS;

                        description += s.ID_PEOPLE == null ? Resources.AUtente + s.GROUP_ID : Resources.ARuolo + s.USER_ID;
                        description += Resources.E;

                        this._dbContext.DeletedSecurityEntities.Remove(s.DeletedSecurity);

                        //inserimento nella security
                        var securityEntity = new SecurityEntity()
                        {
                            THING = thing,
                            PERSONORGROUP = s.DeletedSecurity.PERSONORGROUP,
                            ACCESSRIGHTS = s.DeletedSecurity.ACCESSRIGHTS,
                            ID_GRUPPO_TRASM = s.DeletedSecurity.ID_GRUPPO_TRASM,
                            CHA_TIPO_DIRITTO = "P",
                            HIDE_DOC_VERSIONS = hideDocVersions,
                        };
                    
                        await this._dbContext.SecurityEntities.AddAsync(securityEntity);
                    }

                    if (personOrGroupRU == null && accessRight != null)
                        throw new RipristinaACLPi3Exception();
                }
                else
                {
                    //Diritto rimosso non è quello di proprietario
                    //ricerca di id_gruppo_trasm in deleted_security
                    var inserisciSecurity = true;
                    var deletedSecurityEntity = await this._dbContext.DeletedSecurityEntities
                        .FirstAsync(d => d.THING == thing && d.ACCESSRIGHTS == accessRights && d.PERSONORGROUP == personOrGroup);

                    this._dbContext.DeletedSecurityEntities.Remove(deletedSecurityEntity);

                    //verifica se esistono già diritti in security 
                    //  (questo può accadere quando l'ACL era stata rimossa ma successivamente è stata riacquisita 
                    //  con la propagazione della visibilità a seguito di trasmissioni effettuate da ruoli inferiori)
                    var countACLSecurityEntity = await this._dbContext.SecurityEntities
                        .CountAsync(s => s.THING == thing && s.PERSONORGROUP == personOrGroup && s.ACCESSRIGHTS == accessRights && s.CHA_TIPO_DIRITTO == tipoDiritto);

                    if(countACLSecurityEntity == 0)
                    {
                        var securityEntity = await this._dbContext.SecurityEntities
                            .FirstOrDefaultAsync(s => s.THING == thing && s.PERSONORGROUP == personOrGroup && s.ACCESSRIGHTS == accessRights);

                        if (securityEntity != null && !string.IsNullOrEmpty(securityEntity.CHA_TIPO_DIRITTO))
                        {
                            // è gia inserita una tupla con stesso thing, personorgroup, accessright;
                            // se serve va aggiornata
                            // Priorità del diritto già inserito
                            int priority_cha_tipo_diritto = CalcolaPrioritaChaTipoDiritto(securityEntity.CHA_TIPO_DIRITTO);
                            int nuova_priority_cha_tipo_diritto = CalcolaPrioritaChaTipoDiritto(tipoDiritto);

                            if (priority_cha_tipo_diritto > nuova_priority_cha_tipo_diritto)
                            {
                                inserisciSecurity = false;
                            }
                            else
                            {
                                this._dbContext.SecurityEntities.Remove(securityEntity);
                                inserisciSecurity = true;
                            }
                        }
                        else
                        {
                            // Prelevo l'accessrights
                            var securityEntities = await this._dbContext.SecurityEntities.Where(s => s.THING == thing && s.PERSONORGROUP == personOrGroup).ToListAsync();
                            if(securityEntities != null && securityEntities.Any())
                            {
                                //Se esiste già la coppia thing, personorgroup aggiorno con l'accessrights maggiore
                                securityEntities.ForEach(s =>
                                {
                                    if(s.ACCESSRIGHTS != 20 && accessRights != 20 && accessRights > s.ACCESSRIGHTS)
                                    {
                                        this._dbContext.SecurityEntities.Remove(s);
                                        inserisciSecurity = true;
                                    }
                                });
                            }
                        }

                        if(inserisciSecurity)
                        {
                            var securityEntityToInsert = new SecurityEntity()
                            {
                                THING = thing,
                                PERSONORGROUP = personOrGroup,
                                ACCESSRIGHTS = accessRights,
                                ID_GRUPPO_TRASM = deletedSecurityEntity.ID_GRUPPO_TRASM,
                                CHA_TIPO_DIRITTO = tipoDiritto,
                                HIDE_DOC_VERSIONS = hideDocVersions,
                                CHA_COPIA_VISIBILITA = string.IsNullOrEmpty(request.docDiritto.CopiaVisibilita) ? "0" : request.docDiritto.CopiaVisibilita
                            };

                            await this._dbContext.SecurityEntities.AddAsync(securityEntityToInsert);
                        }
                    }

                    description += request.docDiritto.soggetto.tipoCorrispondente.Equals("R") ? Resources.ARuolo : Resources.AUtente;
                    description += request.docDiritto.soggetto.codiceRubrica;
                }

                if (description.EndsWith(" e "))
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

            return new RipristinaACLResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RipristinaACLHandler> _logger;
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

        private string SetTipoDiritto(DocsPaVO.documento.DirittoOggetto docDir)
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
        protected string GetTipoDiritto(string tipoDiritto)
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
        #endregion
    }
}
