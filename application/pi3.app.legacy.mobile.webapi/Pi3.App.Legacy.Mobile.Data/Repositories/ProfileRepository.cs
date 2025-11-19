// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Linq.Expressions;
using System.Threading;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class ProfileRepository(
    IPi3DbContext pi3DbContext ) : IProfileRepository
{
    private readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<MODELS.SelectTemplates.DettagliDocumento?> GetDocumentoUltimaVersioneByDocNumberAsync(
        long docNumber,
        CancellationToken cancellationToken )
    {
        List<Expression<Func<ProfileEntity, bool>>> conditions =
        [
            x => x.SYSTEM_ID == docNumber,
            x => x.DOCNUMBER == docNumber
        ];

        Expression<Func<ProfileEntity, bool>> orPredicate = Helpers.BuildOrPredicate(conditions);

        IOrderedQueryable<MODELS.SelectTemplates.DettagliDocumento> query = this.GetDettagliDocumentoByCondition(orPredicate);

        MODELS.SelectTemplates.DettagliDocumento? result = await query
            .OrderByDescending( o => o.VersionId )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result;
    }

    public async Task<IEnumerable<MODELS.SelectTemplates.DettagliDocumento>> GetAllegatiDocumentoUltimaVersioneByIdDocumentoPrincipaleAsync(
        long id,
        CancellationToken cancellationToken )
    {
        Expression<Func<ProfileEntity, bool>> predicate = p => p.ID_DOCUMENTO_PRINCIPALE == id;

        IOrderedQueryable<MODELS.SelectTemplates.DettagliDocumento> query = this.GetDettagliDocumentoByCondition(predicate);

        IEnumerable<MODELS.SelectTemplates.DettagliDocumento> result = await query
            .OrderByDescending(o => o.VersionId)
            .ToListAsync(cancellationToken: cancellationToken);

        return result;
    }

    public async Task<MODELS.SelectTemplates.DettagliProtocollo?> GetDettagliProtocolloInArrivoAsync(
        long idProfile,
        CancellationToken cancellationToken )
    {
        MODELS.SelectTemplates.DettagliProtocollo? result = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
            .Join(this._pi3DbContext.DocArrivoParEntities.AsNoTracking(),
                  cg => cg.SYSTEM_ID,
                  dap => dap.ID_MITT_DEST,
                  ( cg, dap ) => new { CorrGlobali = cg, DocArrivoPar = dap })
            .Join(this._pi3DbContext.ProfileEntities.AsNoTracking(),
                  prev => prev.DocArrivoPar.ID_PROFILE,
                  p => p.SYSTEM_ID,
                  ( prev, p ) => new { prev.CorrGlobali, prev.DocArrivoPar, Profile = p })
            .Where(x => x.DocArrivoPar.ID_PROFILE == idProfile 
                        && !string.IsNullOrEmpty(x.DocArrivoPar.CHA_TIPO_MITT_DEST)
                        && x.DocArrivoPar.CHA_TIPO_MITT_DEST.ToUpper().Equals("M") )
            .Select(x => new MODELS.SelectTemplates.DettagliProtocollo
            {
                TipoUrp = x.CorrGlobali.CHA_TIPO_URP,
                Descrizione = x.CorrGlobali.VAR_DESC_CORR,
                Cognome = x.CorrGlobali.VAR_COGNOME,
                Nome = x.CorrGlobali.VAR_NOME
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<IEnumerable<MODELS.SelectTemplates.DettagliProtocollo>?> GetDettagliProtocolloInUscitaAsync( 
        long idProfile, 
        CancellationToken cancellationToken )
    {
        IEnumerable<MODELS.SelectTemplates.DettagliProtocollo>? result = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
            .Join(this._pi3DbContext.DocArrivoParEntities.AsNoTracking(),
                  cg => cg.SYSTEM_ID,
                  dap => dap.ID_MITT_DEST,
                  ( cg, dap ) => new { CorrGlobali = cg, DocArrivoPar = dap })
            .Join(this._pi3DbContext.ProfileEntities,
                  prev => prev.DocArrivoPar.ID_PROFILE,
                  p => p.SYSTEM_ID,
                  ( prev, p ) => new { prev.CorrGlobali, prev.DocArrivoPar, Profile = p })
            .Where(x => x.DocArrivoPar.ID_PROFILE == idProfile 
                    && !string.IsNullOrEmpty(x.DocArrivoPar.CHA_TIPO_MITT_DEST) 
                    && x.DocArrivoPar.CHA_TIPO_MITT_DEST.ToUpper().Equals("D"))
            .Select(x => new MODELS.SelectTemplates.DettagliProtocollo
            {
                TipoUrp = x.CorrGlobali.CHA_TIPO_URP,
                Descrizione = x.CorrGlobali.VAR_DESC_CORR,
                Cognome = x.CorrGlobali.VAR_COGNOME,
                Nome = x.CorrGlobali.VAR_NOME,
                Tipo = x.DocArrivoPar.CHA_TIPO_MITT_DEST
            })
            .ToListAsync(cancellationToken);

        return result;
    }


    #region PRIVATE
    private IOrderedQueryable<MODELS.SelectTemplates.DettagliDocumento> GetDettagliDocumentoByCondition( 
        Expression<Func<ProfileEntity, bool>> predicate)
    {
        IOrderedQueryable<MODELS.SelectTemplates.DettagliDocumento> query = this._pi3DbContext.ProfileEntities.AsNoTracking()
            .Where(predicate)
            .Join(this._pi3DbContext.DocumentTypesEntities.AsNoTracking(),
                  p => p.DOCUMENTTYPE,
                  dt => dt.SYSTEM_ID,
                  ( p, t ) => new { Profile = p, Type = t })
            .Join(_pi3DbContext.ComponentEntities.AsNoTracking(),
                  prev => prev.Profile.DOCNUMBER,
                  c => c.DOCNUMBER,
                  ( prev, c ) => new { prev.Profile, prev.Type, Component = c })
            .Join(_pi3DbContext.VersionEntities.AsNoTracking(),
                  prev => prev.Component.VERSION_ID,
                  v => v.VERSION_ID,
                  ( prev, v ) => new
                  {
                      prev.Profile,
                      prev.Type,
                      prev.Component,
                      Version = v
                  }
            )
            .Select(s => new MODELS.SelectTemplates.DettagliDocumento
            {
                SystemId = s.Profile.SYSTEM_ID,
                NomeOriginale = s.Component.VAR_NOMEORIGINALE,
                Oggetto = s.Profile.VAR_PROF_OGGETTO,
                CreationDate = s.Profile.CREATION_DATE,
                FileSize = s.Component.FILE_SIZE,
                Path = s.Component.PATH, // sul vecchio è FileName
                TipoProto = s.Profile.CHA_TIPO_PROTO,
                Segnatura = s.Profile.VAR_SEGNATURA,
                DaProtocollare = s.Profile.CHA_DA_PROTO,
                DataProtocollazione = s.Profile.DTA_PROTO,
                IdDocumentoPrincipale = s.Profile.ID_DOCUMENTO_PRINCIPALE,

                VersionId = s.Version.VERSION_ID
            })
            .OrderByDescending(o => o.VersionId);

        return query;
    }
    #endregion
}
