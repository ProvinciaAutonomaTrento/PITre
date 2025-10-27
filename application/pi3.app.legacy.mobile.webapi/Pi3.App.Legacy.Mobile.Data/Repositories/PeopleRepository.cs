// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;

using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class PeopleRepository( IPi3DbContext pi3DbContext ) : IPeopleRepository
{
    readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<MODELS.SelectTemplates.UserDetails?> GetUserInformationForClaims( 
        string userId,
        long idAmministrazione,
        long idGroup,
        CancellationToken cancellationToken )
    {
        MODELS.SelectTemplates.UserDetails? result = await this._pi3DbContext.PeopleEntities
            .Join(this._pi3DbContext.AmministraEntities.AsNoTracking(),
                p => p.ID_AMM,
                a => a.SYSTEM_ID,
                (People, Amministrazione) => new { People, Amministrazione }
            )
            .Join(this._pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                prev => prev.People.SYSTEM_ID,
                cg => cg.ID_PEOPLE,
                (prev, cg) => new { prev.People, prev.Amministrazione, CorrGlobali = cg }
            )
            .Join(this._pi3DbContext.PeopleGroupEntities.AsNoTracking(),
                prev => prev.People.SYSTEM_ID,
                pg => pg.PEOPLE_SYSTEM_ID,
                (prev, pg) => new {prev.People, prev.Amministrazione, prev.CorrGlobali, PeopleGroup = pg }
            )
            .Join(this._pi3DbContext.GroupEntities.AsNoTracking(),
                prev => prev.PeopleGroup.GROUPS_SYSTEM_ID,
                g => g.SYSTEM_ID,
                ( prev, g) => new { prev.People, prev.Amministrazione, prev.CorrGlobali, prev.PeopleGroup, Group = g }
            )
            .Join(this._pi3DbContext.CorrGlobaliEntities.AsNoTracking(),
                prev => prev.Group.SYSTEM_ID,
                cg => cg.ID_GRUPPO,
                ( prev, cg ) => new { 
                    prev.People, 
                    prev.Amministrazione,
                    CorrGlobali_P = prev.CorrGlobali, 
                    prev.PeopleGroup, 
                    prev.Group, 
                    CorrGlobali_G = cg 
                }
            )
            .Where(e => 
                e.Amministrazione.SYSTEM_ID == idAmministrazione 
                && e.People.USER_ID == userId
                && e.Group.SYSTEM_ID == idGroup
            )
            .Select(s => new MODELS.SelectTemplates.UserDetails
            {
                Id = s.People.SYSTEM_ID,
                UserId = s.People.USER_ID,
                Cognome = s.People.VAR_COGNOME,
                Nome = s.People.VAR_NOME,
                UserType = s.People.CHA_AMMINISTRATORE,

                AmministrazioneId = s.Amministrazione.SYSTEM_ID,
                AmministrazioneCodice = s.Amministrazione.VAR_CODICE_AMM,
                AmministrazioneDescrizione = s.Amministrazione.VAR_DESC_AMM,

                CorrGlobaliId = s.CorrGlobali_P.SYSTEM_ID,

                GroupSystemId = s.Group.SYSTEM_ID,
                GroupId = s.Group.GROUP_ID,
                GroupName = s.Group.GROUP_NAME,
                GroupCorrGlobaliId = s.CorrGlobali_G.SYSTEM_ID
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result;
    }

    public async Task<IEnumerable<string>> GetUserFunctions(
        long IdRuoloInUo, 
        CancellationToken cancellationToken)
    {
        IEnumerable<string> functions = await this._pi3DbContext.TipoFRuoloEntities.AsNoTracking()
            .Join(this._pi3DbContext.TipoFunzioneEntities.AsNoTracking(),
                prev => prev.ID_TIPO_FUNZ,
                tf => tf.SYSTEM_ID,
                ( prev, tf ) => new { TipoFunzioneRuolo = prev, TipoFunzione = tf }
            )
            .Join(this._pi3DbContext.FunzioneEntities.AsNoTracking(),
                prev => prev.TipoFunzione.SYSTEM_ID,
                f => f.ID_TIPO_FUNZIONE,
                ( prev, f ) => new { prev.TipoFunzioneRuolo, prev.TipoFunzione, Funzione = f }
            )
            .Where( e => e.TipoFunzioneRuolo.ID_RUOLO_IN_UO == IdRuoloInUo)
            .Where( e => e.Funzione.COD_FUNZIONE != null)
            .Select(s => s.Funzione.COD_FUNZIONE!)
            .ToListAsync(cancellationToken);

        return functions;
    }

    public async Task<MODELS.SelectTemplates.Utente?> GetUtenteByUserIdAndIdAdminAsync(
        string userId, 
        long idAmministrazione, 
        CancellationToken cancellationToken )
    {
        MODELS.SelectTemplates.Utente? result = await this._pi3DbContext.PeopleEntities
            .Where(e => userId.Equals(e.USER_ID))
            .Where(e => idAmministrazione.Equals(e.ID_AMM))
            .Select(e => new MODELS.SelectTemplates.Utente
            {
                IdPeople = e.SYSTEM_ID,
                UserId = e.USER_ID,
                IdAmministrazione = e.ID_AMM,
                FullName = e.FULL_NAME
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result;
    }

    public async Task<MODELS.SelectTemplates.Utente?> GetUtenteByUserIdAndIdAdminForLoginAsync(
        string userId,
        long idAmministrazione,
        CancellationToken cancellationToken )
    {
        MODELS.SelectTemplates.Utente? result = await this._pi3DbContext.PeopleEntities
            .Where(e => userId.Equals(e.USER_ID))
            .Where(e => idAmministrazione.Equals(e.ID_AMM))
            .Select(e => new MODELS.SelectTemplates.Utente
            {
                IdPeople = e.SYSTEM_ID,
                UserId = e.USER_ID,
                IdAmministrazione = e.ID_AMM,
                Nome = e.VAR_NOME,
                Cognome = e.VAR_COGNOME,
                Disabled = e.DISABLED,
                EncryptedPassword = e.ENCRYPTED_PASSWORD,
                UserType = e.CHA_AMMINISTRATORE,
                PasswordCreationDate = e.PASSWORD_CREATION_DATE,
                PasswordNeverExpire = e.PASSWORD_NEVER_EXPIRE,
                Memento = e.VAR_MEMENTO
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result;
    }


    public async Task<bool> CheckIfUserIsValidAsync(
        string userId,
        CancellationToken cancellationToken )
    {
        bool userExists = await this._pi3DbContext.PeopleEntities .AsNoTracking()
                                        .AnyAsync(e => e.USER_ID == userId && e.DISABLED != "Y", cancellationToken);

        return userExists;
    }

    public async Task<MODELS.SelectTemplates.Utente?> GetUtenteByIdAsync( 
        long id, 
        CancellationToken cancellationToken )
    {
        MODELS.SelectTemplates.Utente? result = await this._pi3DbContext.PeopleEntities
            .Where(e => e.SYSTEM_ID.Equals(id))
            .Select(e => new MODELS.SelectTemplates.Utente
            {
                IdPeople = e.SYSTEM_ID,
                UserId = e.USER_ID,
                IdAmministrazione = e.ID_AMM,
                FullName = e.FULL_NAME,
                Cognome = e.VAR_COGNOME,
                Nome = e.VAR_NOME
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result;
    }

    public async Task<IEnumerable<long>> GetIdAmministrazioniUtenteByUserIdAsync(
        string userId, 
        CancellationToken cancellationToken )
    {
        IEnumerable<long> result = await this._pi3DbContext.PeopleEntities.AsNoTracking()
            .Where(e => e.USER_ID == userId)
            .Where(e => String.IsNullOrEmpty(e.DISABLED) || !e.DISABLED.ToUpper().Equals("Y"))
            .Where( e=> e.ID_AMM != null)
            .Select(s => s.ID_AMM!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        return result;
    }

}
