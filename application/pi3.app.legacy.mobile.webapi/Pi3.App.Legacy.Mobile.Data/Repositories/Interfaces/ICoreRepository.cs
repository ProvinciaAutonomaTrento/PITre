// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;

public interface ICoreRepository
{
    Task<int> SaveAsync( CancellationToken cancellationToken );
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
