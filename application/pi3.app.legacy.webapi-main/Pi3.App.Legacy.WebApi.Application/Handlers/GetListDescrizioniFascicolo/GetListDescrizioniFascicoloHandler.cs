// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.fascicolazione;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GetListDescrizioniFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListDescrizioniFascicolo;



namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListDescrizioniFascicolo
{
    public class GetListDescrizioniFascicoloHandler : IRequestHandler<GetListDescrizioniFascicoloRequest, GetListDescrizioniFascicoloResult>
    {

        protected readonly ILogger<GetListDescrizioniFascicoloHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        private async Task<int> GetListDescrizioniFascicoloCount(Expression<Func<DescrizioneFascEntity,bool>> condition, string idAmm)
        {
            int nRec = 0;
            nRec = await this._dbContext.DescrizioneFascEntities.AsNoTracking().Where(d => d.ID_AMM == idAmm.AsLong()).Where(condition).CountAsync();

            return nRec;

        }


        private class InfoListaDesc
        {
            public long SYSTEM_ID { get; set; }
            public string? VAR_DESCRIZIONE { get; set; }
            public string? VAR_CODICE { get; set; }
            public long? ID_REGISTRO { get; set; }
            public long? ID_AMM { get; set; }
            public string? CODICE_REGISTRO { get; set; }
        }

        private async Task<GetListDescrizioniFascicoloResult> GetListDescrizioniFascicolo(List<DocsPaVO.fascicolazione.FiltroDescrizioniFascicolo> filters, DocsPaVO.utente.InfoUtente infoUtente, int numPage, int pageSize)
        {
            List<DescrizioneFascicolo> listDescFasc = new List<DescrizioneFascicolo>();
            int numTotPage = 0, nRec = 0;

            var listaDescFascCountCondition = PredicateBuilder.New<DescrizioneFascEntity>();
            var listaDescFascCondition = PredicateBuilder.New<InfoListaDesc>();

            foreach (FiltroDescrizioniFascicolo f in filters)
            {
                switch (f.Argomento)
                {

                    case "CODICE":
                        listaDescFascCountCondition = listaDescFascCountCondition.And( d => d.VAR_CODICE != null && d.VAR_CODICE.ToUpper().Contains(f.Valore.ToUpper().Replace("'", "''")) );
                        listaDescFascCondition = listaDescFascCondition.And( d => d.VAR_CODICE != null && d.VAR_CODICE.ToUpper().Contains(f.Valore.ToUpper().Replace("'", "''")) );
                        break;
                    case "DESCRIZIONE":
                        listaDescFascCountCondition = listaDescFascCountCondition.And(d => d.VAR_DESCRIZIONE != null && d.VAR_DESCRIZIONE.ToUpper().Contains(f.Valore.ToUpper().Replace("'", "''")));
                        listaDescFascCondition = listaDescFascCondition.And(d => d.VAR_DESCRIZIONE != null && d.VAR_DESCRIZIONE.ToUpper().Contains(f.Valore.ToUpper().Replace("'", "''")));
                        break;
                    case "REGISTRO":
                        List<string> listaIdRegRf = f.Valore.Split('_').ToList();
                        listaDescFascCountCondition = listaDescFascCountCondition.And( d => d.ID_REGISTRO != null && listaIdRegRf.Contains(d.ID_REGISTRO.ToString()));
                        listaDescFascCondition = listaDescFascCondition.And( d => d.ID_REGISTRO != null && listaIdRegRf.Contains(d.ID_REGISTRO.ToString()));
                        break;


                }
            }


            nRec = await this.GetListDescrizioniFascicoloCount(listaDescFascCountCondition, infoUtente.idAmministrazione);


            if (nRec > 0)
            {
                numTotPage = (nRec / pageSize);
                int startRow = ((numPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;
                string paging = string.Empty;

                var subQuery2 = (from d in this._dbContext.DescrizioneFascEntities.AsNoTracking()
                             join r in this._dbContext.RegistroEntities.AsNoTracking() on d.ID_REGISTRO equals r.SYSTEM_ID into ct
                             from t in ct.DefaultIfEmpty()
                             where d.ID_AMM == infoUtente.idAmministrazione.AsLong()
                             orderby d.VAR_DESCRIZIONE ascending
                             select new InfoListaDesc()
                             {
                                 SYSTEM_ID = d.SYSTEM_ID,
                                 VAR_DESCRIZIONE =d.VAR_DESCRIZIONE,
                                 VAR_CODICE = d.VAR_CODICE,
                                 ID_REGISTRO = d.ID_REGISTRO,
                                 ID_AMM = d.ID_AMM,
                                 CODICE_REGISTRO = t.VAR_CODICE
                             });

                subQuery2 = subQuery2.Where(listaDescFascCondition);


                var subQuery1 = subQuery2.Select(
                    (ent, row_num) => new 
                    { 
                        ent,
                        rnum = row_num+1
                    }).Where( ent => ent.rnum <= endRow && ent.rnum >= startRow);

                DescrizioneFascicolo desc = new();

                foreach ( var row in subQuery1 )
                {
                    desc = new DescrizioneFascicolo();
                    desc.SystemId = row.ent.SYSTEM_ID.ToString();
                    desc.IdAmm = row.ent.ID_AMM?.ToString();
                    desc.Descrizione = !string.IsNullOrEmpty(row.ent.VAR_DESCRIZIONE?.ToString()) ? row.ent.VAR_DESCRIZIONE.ToString() : string.Empty;
                    desc.Codice = !string.IsNullOrEmpty(row.ent.VAR_CODICE?.ToString()) ? row.ent.VAR_CODICE.ToString() : string.Empty;
                    desc.IdRegistro = !string.IsNullOrEmpty(row.ent.ID_REGISTRO?.ToString()) ? row.ent.ID_REGISTRO.ToString() : string.Empty;
                    desc.CodRegistro = !string.IsNullOrEmpty(row.ent.CODICE_REGISTRO?.ToString()) ? row.ent.CODICE_REGISTRO.ToString() : string.Empty;
                    listDescFasc.Add(desc);

                }
            }
            return new(listDescFasc,numTotPage,nRec);
        }






        public GetListDescrizioniFascicoloHandler(
            ILogger<GetListDescrizioniFascicoloHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        public async Task<GetListDescrizioniFascicoloResult> Handle(GetListDescrizioniFascicoloRequest request, CancellationToken cancellationToken)
        {
            List<DescrizioneFascicolo> output = new List<DescrizioneFascicolo>();
            int numTotPage = 0 , nRec = 0;
            GetListDescrizioniFascicoloResult result = new GetListDescrizioniFascicoloResult(output, numTotPage, nRec);

            try
            {
                result = await this.GetListDescrizioniFascicolo(request.filters,request.infoUtente,request.numPage,request.pageSize);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);

            }

            return result;

        }

    }
}
