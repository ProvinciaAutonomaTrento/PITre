// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using pi3.Core.Contracts.DocumentoAmministrativo;

namespace Pi3.App.Indexer.Infrastructure.Services.Indexer
{
    public class MockIndexerService : IIndexingService
    {
        public MockIndexerService()
        {
                
        }

        public async Task<bool> AddOrEditDocument<T> (T document, string index) => true;


        public async Task<bool> DeleteDocument(string id, string index) => true;

    }
}
