// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AddressbookGetRuoliUtenteIntRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetRuoliUtenteInt;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetRuoliUtenteInt
{
    public class AddressbookGetRuoliUtenteIntHandler : IRequestHandler<AddressbookGetRuoliUtenteIntRequest, AddressbookGetRuoliUtenteIntResult>
    {
        protected readonly IPi3DbContext _dbContext;
        protected readonly ILogger<AddressbookGetRuoliUtenteIntHandler> _logger;

        public AddressbookGetRuoliUtenteIntHandler(
            IPi3DbContext dbContext,
            ILogger<AddressbookGetRuoliUtenteIntHandler> logger
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


      

        public async Task<AddressbookGetRuoliUtenteIntResult> Handle(AddressbookGetRuoliUtenteIntRequest request, CancellationToken cancellationToken)
        {
            DataSet? output = null;

            try
            {

                var ruoliUtente = await (from u in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from p in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from r in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                               from pg in this._dbContext.PeopleGroupEntities.AsNoTracking()
                               from e in this._dbContext.PeopleEntities.AsNoTracking()
                               where r.ID_TIPO_RUOLO == b.SYSTEM_ID
                               && (p.CHA_TIPO_URP != null ? p.CHA_TIPO_URP!.Equals("P") : false)
                               && (p.CHA_TIPO_IE != null ? p.CHA_TIPO_IE!.Equals("I") : false)
                               && (p.CHA_TIPO_CORR != null ? p.CHA_TIPO_CORR!.Equals("S") : false)
                               && (r.CHA_TIPO_URP != null ? r.CHA_TIPO_URP!.Equals("R") : false)
                               && (r.CHA_TIPO_IE != null ? r.CHA_TIPO_IE!.Equals("I") : false)
                               && (r.CHA_TIPO_CORR != null ? r.CHA_TIPO_CORR!.Equals("S") : false)
                               && (u.CHA_TIPO_IE != null ? u.CHA_TIPO_IE!.Equals("I") : false)
                               && (u.CHA_TIPO_URP != null ? u.CHA_TIPO_URP!.Equals("U") : false)
                               && e.SYSTEM_ID == p.ID_PEOPLE
                               && pg.GROUPS_SYSTEM_ID == r.ID_GRUPPO
                               && pg.PEOPLE_SYSTEM_ID == p.ID_PEOPLE
                               && r.ID_UO == u.SYSTEM_ID
                               && !pg.DTA_FINE.HasValue
                               && (p.VAR_COD_RUBRICA != null ? p.VAR_COD_RUBRICA!.ToUpper().Equals(request.codRubrica.ToUpper()) : false)
                               orderby pg.CHA_PREFERITO descending
                               select new
                               {
                                   e.VAR_COGNOME,
                                   ruolo_system_id = r.SYSTEM_ID,
                                   ruolo_desc = r.VAR_DESC_CORR,
                                   ruolo_codice = r.VAR_CODICE,
                                   ruolo_cod_rubrica = r.VAR_COD_RUBRICA,
                                   descrizioneUO = u.VAR_DESC_CORR,
                                   cha_preferito = pg.CHA_PREFERITO
                               }).ToListAsync();

                output = ruoliUtente.AsDataSet();

                if (output == null)
                {
                    throw new Exception();
                }

            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new AddressbookGetRuoliUtenteIntResult(output);
        }


    }
}
