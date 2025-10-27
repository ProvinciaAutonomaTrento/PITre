// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocumentFormat.OpenXml.EMMA;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getNodoTitolarioByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.getNodoTitolarioById;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getNodoTitolarioById
{
    public class getNodoTitolarioByIdHandler : IRequestHandler<getNodoTitolarioByIdRequest, getNodoTitolarioByIdResult>
    {
        protected readonly ILogger<getNodoTitolarioByIdHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        public getNodoTitolarioByIdHandler(
            ILogger<getNodoTitolarioByIdHandler> logger,
            IPi3DbContext dbContext,
            IConfigurationService configurationService
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }


        public async Task<getNodoTitolarioByIdResult> Handle(getNodoTitolarioByIdRequest request, CancellationToken cancellationToken)
        {
            OrgNodoTitolario nodoTitolario = null;

            try
            {
                var nodes = await (from p in this._dbContext.ProjectEntities.AsNoTracking()
                 from a in this._dbContext.AmministraEntities.AsNoTracking()
                 where a.SYSTEM_ID == p.ID_AMM && p.SYSTEM_ID == request.idNodoTitolario.AsLong()
                 select new
                 {
                     p,
                     a
                 }).ToListAsync();
                var nodoEnt = nodes != null && nodes.Count>0? nodes[0]:null;
                if (nodoEnt != null)
                {
                    nodoTitolario = new DocsPaVO.amministrazione.OrgNodoTitolario();
                    nodoTitolario.ID = nodoEnt.p.SYSTEM_ID.ToString();
                    nodoTitolario.bloccaTipoFascicolo = nodoEnt.p.CHA_BLOCCA_FASC;
                    nodoTitolario.Codice = nodoEnt.p.VAR_CODICE;
                    nodoTitolario.CodiceAmministrazione = nodoEnt.a.VAR_CODICE_AMM;
                    nodoTitolario.CodiceLivello = nodoEnt.p.VAR_COD_LIV1;
                    nodoTitolario.Descrizione = nodoEnt.p.DESCRIPTION;
                    nodoTitolario.ID_TipoFascicolo = nodoEnt.p.ID_TIPO_FASC != null ? nodoEnt.p.ID_TIPO_FASC.ToString() : null;
                    nodoTitolario.IDParentNodoTitolario = nodoEnt.p.ID_PARENT != null ? nodoEnt.p.ID_PARENT.ToString() : null;
                    nodoTitolario.ID_Titolario = nodoEnt.p.ID_TITOLARIO != null ? nodoEnt.p.ID_TITOLARIO.ToString() : null;
                    nodoTitolario.IDRegistroAssociato = nodoEnt.p.ID_REGISTRO != null ? nodoEnt.p.ID_REGISTRO.ToString() : null;
                    nodoTitolario.Livello = nodoEnt.p.NUM_LIVELLO != null ? nodoEnt.p.NUM_LIVELLO.ToString() : null;
                    if (nodoEnt.p.NUM_MESI_CONSERVAZIONE.Equals(""))
                        nodoTitolario.NumeroMesiConservazione = 0;
                    else
                        nodoTitolario.NumeroMesiConservazione = Convert.ToInt32(nodoEnt.p.NUM_MESI_CONSERVAZIONE.ToString());
                    nodoTitolario.note = nodoEnt.p.NUM_MESI_CONSERVAZIONE != null ? nodoEnt.p.NUM_MESI_CONSERVAZIONE.ToString() : string.Empty;

                    nodoTitolario.numProtoTit = nodoEnt.p.NUM_PROT_TIT;
                    nodoTitolario.contatoreAttivo = nodoEnt.p.CHA_CONTA_PROT_TIT != null? nodoEnt.p.CHA_CONTA_PROT_TIT.ToString() : null;
                    nodoTitolario.bloccaNodiFigli = nodoEnt.p.CHA_BLOCCA_FIGLI != null? nodoEnt.p.CHA_BLOCCA_FIGLI.ToString() : null;
                    nodoTitolario.dataCreazione = nodoEnt.p.DTA_CREAZIONE.HasValue ? nodoEnt.p.DTA_CREAZIONE.ToString() : null;
                    if (nodoEnt.p.CHA_RW != null)
                    {
                        if (nodoEnt.p.CHA_RW.ToString() == "W")
                            nodoTitolario.CreazioneFascicoliAbilitata = true;
                        else
                            nodoTitolario.CreazioneFascicoliAbilitata = false;
                    }


                }   
                
            }
            catch( Exception ex )
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(nodoTitolario);
        }
    }
}
