// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.App.Indexer.Infrastructure.Services.Indexer;

namespace Pi3.App.Indexer.Infrastructure.AggregateModels.DocumentBlobAggregate
{

    public class DocumentBlobMockRepository : ElementRepository<DocumentBlob>, IDocumentBlobRepository
    {
        #region Public Members

        public DocumentBlobMockRepository(ILogger<DocumentBlobMockRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IOptions<DocumentBlobMockRepositoryOptions> options)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            this._options = options;
        }

        #endregion

        #region Private Members

        private IOptions<DocumentBlobMockRepositoryOptions> _options;

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            return true;
        }

        protected override async Task<DocumentBlob> HandleGet(DocumentBlob newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var fileName = $"{nameof(Files.TestFile)}.pdf";

            //id = this._options.Value.MockId;

            var aggregate = new DocumentBlob(id, idTenant, DateTime.Now, new(fileName));
            aggregate.LoadFileName(fileName);
            aggregate.LoadStream(new MemoryStream(Files.TestFile));
            aggregate.MarkChangesAsCommitted();

            return aggregate;
        }

        protected override async Task HandleAdd(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleDelete(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleUpdate(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
