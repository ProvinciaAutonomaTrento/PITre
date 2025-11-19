// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.StampaRegistri;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Extensions
{
    internal static class QueryableExtensions
    {
        public static IQueryable<ReportRegistriItem> AsReportRegistriItemQueryable(this IQueryable<ProfileEntity> queryable, IPi3DbContext dbContext)
        {
            return queryable.Select(x => new ReportRegistriItem
            {
                Docnumber = x.SYSTEM_ID,
                RecordNumber = x.NUM_PROTO,
                RecordDate = x.DTA_PROTO,
                RecordType = x.CHA_TIPO_PROTO,
                CancellationDate = x.DTA_ANNULLA,
                Subject = x.VAR_PROF_OGGETTO,
                SenderRecipients = IPi3DbContextMappedFunctions.CorrCat(x.SYSTEM_ID, x.CHA_TIPO_PROTO!),
                Folders = IPi3DbContextMappedFunctions.ClassCat(x.SYSTEM_ID),
                Hash = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.SYSTEM_ID),
                IsSubjectModified = !(IPi3DbContextMappedFunctions.IsOggettoModificato(x.SYSTEM_ID) == "0"),
                IsSenderOrRecipientsModified = dbContext.CorrStoEntities.AsNoTracking().Any(y => y.ID_PROFILE == x.SYSTEM_ID && y.DTA_MODIFICA > x.DTA_PROTO),
                AttachmentsNumber = IPi3DbContextMappedFunctions.CountAllegatiByDocNumber(x.SYSTEM_ID)
            });
        }
    }
}
