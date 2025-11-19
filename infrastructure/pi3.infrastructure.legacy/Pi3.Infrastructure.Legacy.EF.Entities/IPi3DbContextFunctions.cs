// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public interface IPi3DbContextFunctions
    {
        Task<SecurityRightTypesEnum> GetSecurityRights(string thing, string idUser, string? idGroup = null);
        Task<SecurityEntity> GetSecurity(string thing, string idUser, string? idGroup = null);

        Task BookRegProto(long idRegistro);

        Task BookProgressivoFascicolo(long idTitolario, long? idRegistro = null);

        Task<DateTime> GetSystemDateTime();

        Task<IReadOnlyList<CorrGlobaliEntity>> GetHierarcy(string idGroup);

        Task<IReadOnlyList<CorrGlobaliEntity>> GetChildren(string id);

        Task<string> ConvertDegre();
        Task<long> GetValCampoProfDocOrderWithNumberCast(long docNumber, long objId);
        Task<DateTime> GetValCampoProfDocWithDataCast(long docNumber, long objId);
        Task BookContatoreRepertorio(long idContatore);

        Task BookContatoreRepertorioComune(long idContatore);

        Task BookContatoreRepertorioFasc(long idContatore);

        IQueryable<ProfileFullTextEntity> ProfileFullText(string text);
        IQueryable<CorrGlobaliFullTextEntity> CorrGlobaliFullText(string text);
    }
}
