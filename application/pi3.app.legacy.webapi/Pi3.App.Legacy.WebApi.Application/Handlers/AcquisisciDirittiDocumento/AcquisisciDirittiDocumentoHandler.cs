// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AcquireRightsFromExtSys;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcquisisciDirittiDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.AcquisisciDirittiDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AcquisisciDirittiDocumento
{
    public class AcquisisciDirittiDocumentoHandler : IRequestHandler<AcquisisciDirittiDocumentoRequest, AcquisisciDirittiDocumentoResult>
    {
        #region Public Members

        public AcquisisciDirittiDocumentoHandler(ILogger<AcquisisciDirittiDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<AcquisisciDirittiDocumentoResult> Handle(AcquisisciDirittiDocumentoRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var thing = request.schedaDocumento.docNumber.AsLong();
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var proprietarioOld = await this._dbContext.PeopleEntities
                    .Where(p => p.SYSTEM_ID == request.schedaDocumento.autore.AsLong() && p.ID_AMM == idTenant.AsLong())
                    .Select(p => p.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var aggregate = await this._repository.Get(idTenant, request.schedaDocumento.docNumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = false
                    }
                });
                aggregate.ChangeTipoVisibilita(Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.TipologieVisibilitaEnum.Gerarchica);
                await this._repository.Update(aggregate);

                //Elimino il vecchio proprietario dalla Security
                if (proprietarioOld != null)
                {
                    var securityProprietarioEntity = await this._dbContext.SecurityEntities
                        .FirstOrDefaultAsync(s => s.THING == thing && s.PERSONORGROUP == proprietarioOld);
                    if (securityProprietarioEntity != null)
                        this._dbContext.SecurityEntities.Remove(securityProprietarioEntity);

                    //Aggiungo il vecchio proprietario nella DeletedSecurity
                    var deletedSecurityProprietarioEntity = new DeletedSecurityEntity()
                    {
                        THING = thing,
                        PERSONORGROUP = proprietarioOld,
                        ACCESSRIGHTS = 0,
                        ID_GRUPPO_TRASM = null,
                        CHA_TIPO_DIRITTO = "P",
                        NOTE = Resources.DirittoCedutoDaTIBCO,
                        DTA_REVOCA = await this._dbContext.GetSystemDateTime(),
                        ID_UTENTE_REV = idUser,
                        ID_RUOLO_REV = idGroup,
                        HIDE_DOC_VERSIONS = null
                    };
                    await this._dbContext.DeletedSecurityEntities.AddAsync(deletedSecurityProprietarioEntity);
                }

                //Eliminazione del nuovo ruolo dalla Security
                var securityNuovoProprietarioEntityToDelete = await this._dbContext.SecurityEntities
                    .FirstOrDefaultAsync(s => s.THING == thing && s.PERSONORGROUP == idGroup);
                if (securityNuovoProprietarioEntityToDelete != null)
                    this._dbContext.SecurityEntities.Remove(securityNuovoProprietarioEntityToDelete);

                //Aggiungo il nuovo proprietario nella Security
                var securityNuovoUtenteProprietarioEntityToInsert = new SecurityEntity()
                {
                    THING = thing,
                    PERSONORGROUP = idUser,
                    ACCESSRIGHTS = 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P",
                    HIDE_DOC_VERSIONS = null
                };
                await this._dbContext.SecurityEntities.AddAsync(securityNuovoUtenteProprietarioEntityToInsert);

                //Aggiungo il ruolo del nuovo proprietario nella Security
                var securityNuovoRuoloProprietarioEntityToInsert = new SecurityEntity()
                {
                    THING = thing,
                    PERSONORGROUP = idGroup,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P",
                    HIDE_DOC_VERSIONS = null
                };
                await this._dbContext.SecurityEntities.AddAsync(securityNuovoRuoloProprietarioEntityToInsert);

                //Aggiungo utente TIBCO il diritto di WRITE nella Security
                var idPeopleTIBCO = await this._dbContext.PeopleEntities
                    .Where(p => p.USER_ID.ToUpper() == "TIBCO" && p.ID_AMM == idTenant.AsLong())
                    .Select(p => p.SYSTEM_ID)
                    .FirstOrDefaultAsync();
                if (idPeopleTIBCO != null)
                {
                    var securityUtenteTibcoEntityToInsert = new SecurityEntity()
                    {
                        THING = thing,
                        PERSONORGROUP = idPeopleTIBCO,
                        ACCESSRIGHTS = 63,
                        ID_GRUPPO_TRASM = null,
                        CHA_TIPO_DIRITTO = "A",
                        HIDE_DOC_VERSIONS = null
                    };
                    await this._dbContext.SecurityEntities.AddAsync(securityUtenteTibcoEntityToInsert);
                }

                await ((DbContext)_dbContext).SaveChangesAsync();

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new AcquisisciDirittiDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AcquisisciDirittiDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
