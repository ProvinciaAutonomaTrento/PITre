// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;

namespace Pi3.App.DocumentoAmministrativo.WebApi
{
    public class DocumentBlobFakeRepository : ElementRepository<DocumentBlob>, IDocumentBlobRepository
    {
        #region Public Members

        public DocumentBlobFakeRepository(
            ILogger<DocumentBlobFakeRepository> logger,
            IClaimsPrincipalService claimsPrincipal,
            IEventPublisher eventPublisher)
                  : base(logger, claimsPrincipal, eventPublisher)
        {
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            return true;
        }

        protected override async Task<DocumentBlob> HandleGet(string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            id = @"C:\DocsPaContentServer\DocServer\DEMO1\2020\PRO UPROT\42130.pdf";

            var fileInfo = new FileInfo(id);

            var aggregate = new DocumentBlob(id, idTenant, fileInfo.CreationTime, new(fileInfo.Name));
            aggregate.LoadFileContent(fileInfo.Name, await File.ReadAllBytesAsync(id));
            aggregate.MarkChangesAsCommitted();
            return aggregate;
        }

        #endregion

        #region Private Members

        protected override async Task RequestUpdate(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task RequestDelete(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task RequestAdd(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
