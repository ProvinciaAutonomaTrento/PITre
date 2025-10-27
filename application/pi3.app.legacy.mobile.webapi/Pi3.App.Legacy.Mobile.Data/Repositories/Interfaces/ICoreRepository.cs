// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;

public interface ICoreRepository
{
    Task<int> SaveAsync( CancellationToken cancellationToken );
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
