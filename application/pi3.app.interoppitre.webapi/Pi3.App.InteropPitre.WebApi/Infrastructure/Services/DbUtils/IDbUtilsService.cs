// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Repository
{
    public interface IDbUtilsService
    {
        Task InsertLog(string docnumber, string message, long error);

        Task<IEnumerable<string>> GetCorrespondentsId(string code);
    }
}
