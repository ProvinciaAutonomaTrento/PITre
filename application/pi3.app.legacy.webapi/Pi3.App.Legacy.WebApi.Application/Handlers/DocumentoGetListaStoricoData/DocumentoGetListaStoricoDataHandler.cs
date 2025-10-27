// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Repositories;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetListaStoricoDataRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetListaStoricoData;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetListaStoricoData
{
    public class DocumentoGetListaStoricoDataHandler : IRequestHandler<DocumentoGetListaStoricoDataRequest, DocumentoGetListaStoricoDataResult>
    {
        protected readonly ILogger<DocumentoGetListaStoricoDataHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IRuoloCorrispondenteRepository _ruoloCorrispondenteRepository;
        protected readonly IPersonaCorrispondenteRepository _personaCorrispondenteRepository;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        private DocsPaVO.utente.Utente GetUtente(string idAmm, string idPeople)
        {
            var pc = _personaCorrispondenteRepository.Get(idAmm.ToString(), idPeople).Result;
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
                idPeople = idPeople,
                idRegistro = pc.IdRegistro,
                dta_fine = pc.DataFine.AsDateFormat(),
                descrizione = pc.Description.ToString(),

            };
            return u;
        }

        private Ruolo GetRuolo(string idAmm,string idGroup)
        {
            var rc = _ruoloCorrispondenteRepository.Get(idAmm, idGroup).Result;
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
            };
            return r;
        }


        private bool FromCharToBool(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("1"))
                return true;
            else
                return false;
        }

        public DocumentoGetListaStoricoDataHandler(
            ILogger<DocumentoGetListaStoricoDataHandler> logger,
            IPi3DbContext dbContext,
            IRuoloCorrispondenteRepository ruoloCorrispondenteRepository,
            IPersonaCorrispondenteRepository personaCorrispondenteRepository,
            IClaimsPrincipalService claimsPrincipalService
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._ruoloCorrispondenteRepository = ruoloCorrispondenteRepository;
            this._personaCorrispondenteRepository = personaCorrispondenteRepository;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<DocumentoGetListaStoricoDataResult> Handle(DocumentoGetListaStoricoDataRequest request, CancellationToken cancellationToken)
        {
            List<DocumentoStoricoDataArrivo> output = new();
            try
            {
                string idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                var storico = await (from sto in this._dbContext.DataArrivoStoEntities.AsNoTracking()
                where sto.DOCNUMBER == request.docnumber.AsLong()
                orderby sto.DTA_MODIFICA descending
                select new
                {

                    sto.SYSTEM_ID,
                    sto.ID_PEOPLE,
                    sto.ID_GROUP,
                    sto.DTA_ARRIVO,
                    sto.DTA_MODIFICA
                }).ToListAsync();

                foreach( var sto in storico )
                {
                    DocumentoStoricoDataArrivo st = new();
                    st.systemId = sto.SYSTEM_ID.ToString();
                    st.utente = this.GetUtente(idAmm,sto.ID_PEOPLE.ToString());
                    st.ruolo = this.GetRuolo(idAmm,sto.ID_GROUP.ToString());
                    st.dta_arrivo = sto.DTA_ARRIVO.AsDateFormat();
                    st.dataModifica = sto.DTA_MODIFICA.AsDateFormat();
                    output.Add(st);
                }
            }
            catch ( Exception ex )
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output.ToArray());
        }
    }
}
