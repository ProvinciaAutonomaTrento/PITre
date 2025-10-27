// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Repository
{
    public class DbUtilsService : IDbUtilsService
    {
        private readonly IPi3DbContext _dbContext;
        public DbUtilsService(IPi3DbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IEnumerable<string>> GetCorrespondentsId(string code)
        {
            var idList = new List<string>();

            foreach (string c in code.Split(","))
            {
                //var id = await this._dbContext.CorrGlobaliEntities.Where(x => x.VAR_COD_RUBRICA == c.Replace("'", ""))
                //    .Select(x => x.SYSTEM_ID)
                //    .FirstOrDefaultAsync();

                var idCorrs= await this._dbContext.CorrGlobaliEntities.Where(x => x.VAR_COD_RUBRICA == c.Replace("'", ""))
                    .Select(x => x.SYSTEM_ID)
                    .ToListAsync();

                if (idCorrs.Any())
                {
                    foreach (var idCorr in idCorrs)
                    {
                        var id = idCorr;
                        while (id != 0)
                        {
                            idList.Add(id.ToString());
                            id = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == id)
                                .Select(x => x.ID_OLD.Value)
                                .FirstOrDefaultAsync();
                        }
                    }
                }
            }

            return idList;
        }

        public async Task InsertLog(string docnumber, string message, long error)
        {
            await ((DbContext)this._dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({docnumber}, {error}, {message})");
        }
    }
}
