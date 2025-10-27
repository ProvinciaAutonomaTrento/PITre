// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocumentFormat.OpenXml.Office2016.Word.Symex;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GiveUpRights
{
    // Richiede libreria MediatR
    public class GiveUpRightsCommandHandler : IRequestHandler<GiveUpRightsCommand, GiveUpRightsCommandResponse>
    {
        #region Public Members

        public GiveUpRightsCommandHandler(ILogger<GiveUpRightsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GiveUpRightsCommandResponse> Handle(GiveUpRightsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GiveUpRights - START");

            GiveUpRightsCommandResponse response = new GiveUpRightsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                //TODO
                int idOggetto;

                var rightToKeep = request.RightToKeep;

                if (string.IsNullOrEmpty(rightToKeep))
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                else if (rightToKeep.ToUpper() != "WRITE" && rightToKeep.ToUpper() != "READ" && rightToKeep.ToUpper() != "NONE")
                {
                    throw new Exception("Wrong value for parameter RightToKeep. Accepted values: WRITE, READ, NONE.");
                }
                if (string.IsNullOrEmpty(request.IdObject))
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                else if (!Int32.TryParse(request.IdObject, out idOggetto))
                {
                    throw new Exception("IdObject must be an integer");
                }

                var idObject = request.IdObject;
                DocsPaVO.amministrazione.SistemaEsterno sysExt = await this.GetSistemaEsternoByUserid(infoUtente.idAmministrazione, infoUtente.userId);

                bool trovato = false;

                string accessRightsUtente, idGruppoTrasmUtente, TipoDirittoUtente;
                string accessRightsRuolo = string.Empty, idGruppoTrasmRuolo = string.Empty, TipoDirittoRuolo = string.Empty;
                //SelectSecurity DA FINIRE
                //trovato = this.SelectSecurity(out accessRightsUtente, out idGruppoTrasmUtente, out TipoDirittoUtente, idObject, utente.idPeople, null);

                //if (trovato && !string.IsNullOrEmpty(accessRightsUtente) && !string.IsNullOrEmpty(TipoDirittoUtente))
                //    trovato = this.SelectSecurity(out accessRightsRuolo, out idGruppoTrasmRuolo, out TipoDirittoRuolo, idObject, ruolo.idGruppo, null);
                #endregion

                response.Code = MessageResponseCode.OK;

                _logger.LogInformation("end GiveUpRights");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GiveUpRights: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GiveUpRightsCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GiveUpRights");
                response = new GiveUpRightsCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        //DA FINIRE
        //private async bool SelectSecurity(out string accessRight, out string idGruppoTrasm, out string tipoDiritto, string idObject, string idPeopleOrGroup, string accessRightsToTest)
        //{
        //    bool output = false;
        //    accessRight = string.Empty;
        //    idGruppoTrasm = string.Empty;
        //    tipoDiritto = string.Empty;

        //    try
        //    {

        //        long thing = idObject.AsLong();
        //        long person_or_groups = idPeopleOrGroup.AsLong();
        //        var entities = await this.GetEntitities(thing, person_or_groups, accessRightsToTest);





        //        entities.ForEach(x =>
        //        {
        //            accessRight = x.ACCESSRIGHTS.ToString(),
        //            idGruppoTrasm = x.ID_GRUPPO_TRASM.ToString(),
        //            tipoDiritto = x.CHA_TIPO_DIRITTO,
        //            output = true
        //        }
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, null, null);
        //    }

        //}

        private async Task<List<SecurityEntity>> GetEntitities(long thing, long person_or_groups, string accessRightsToTest)
        {

            var entities = _pi3DbContext.SecurityEntities.Where(s => s.THING == thing && s.PERSONORGROUP == person_or_groups);
            if (!string.IsNullOrEmpty(accessRightsToTest))
            {
                long accessrights = Convert.ToInt64(accessRightsToTest);
                entities = entities.Where(s => s.ACCESSRIGHTS == accessrights);
            }

            var result = await entities.Select(x => new SecurityEntity
            {
                ACCESSRIGHTS = x.ACCESSRIGHTS,
                ID_GRUPPO_TRASM = x.ID_GRUPPO_TRASM,
                CHA_TIPO_DIRITTO = x.CHA_TIPO_DIRITTO,

            }
                ).ToListAsync();
            return result;

        }

        private async Task<SistemaEsterno> GetSistemaEsternoByUserid(string idAmministrazione, string userId)
        {

            SistemaEsterno retval;

            retval = await this._pi3DbContext.ExternalSystemEntities.Where(es => es.ID_AMM == idAmministrazione.AsLong() && es.VAR_USER_ID.ToUpper() == userId.ToUpper())
            .Select(es => new SistemaEsterno
            {
                IdSistemaEsterno = es.SYSTEM_ID.ToString(),
                idAmministrazione = es.ID_AMM.ToString(),
                CodiceApplicazione = es.VAR_CODE_APPLICATION,
                DescEstesa = es.VAR_DESC_ESTESA ?? string.Empty,
                Diritti = es.VAR_PIS_METHODS_ALLOWED,
                idRuoloAssociato = es.ID_SYSTEM_ROLE.ToString(),
                UserIdAssociato = es.VAR_USER_ID,
                TokenPeriod = Int32.Parse(es.VAR_TKN_TIME)
            }).FirstOrDefaultAsync();


            if (retval == null)
            {
                throw new Exception("Metodo non disponibile per l'utente: l'utente non è un sistema esterno");
            }


            return retval;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GiveUpRightsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
