// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services.Principal;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Pi3.Core.Services.Configuration;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Events;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.Extensions;

namespace Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate
{
    public class DocumentBlobFileSystemRepository : ElementRepository<DocumentBlob>, IDocumentBlobRepository
    {
        #region Public Members

        public DocumentBlobFileSystemRepository(
            ILogger<DocumentBlobFileSystemRepository> logger,
            IClaimsPrincipalService claimsPrincipal,
            IEventPublisher eventPublisher,
            IConfigurationService configurationService)
                  : base(logger, claimsPrincipal, eventPublisher)
        {
            this._configurationService = configurationService;
        }

        #endregion

        #region Private Members

        protected readonly IConfigurationService _configurationService;
        protected const int _defaultBufferSize = 1024;

        protected virtual async Task<string> GetRootPath()
        {
            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

            this._logger.LogDebug($"GetRootPath: {repositoryRootPath}");

            return repositoryRootPath;
        }

        protected virtual async Task AssertFileAlreadyExists(DocumentBlob aggregate)
        {
            if (File.Exists(aggregate.Id.PathAsUnixPath()))
                throw new DocumentBlobAlreadyExistsPi3Exception(aggregate.Id);
        }

        protected virtual async Task GenerateId(DocumentBlob aggregate)
        {
            var id = string.Concat(
                    await this.GetRootPath(),
                    @"\",
                    this._claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true).ToUpper(),
                    @"\",
                    aggregate.CreationDate.Year.ToString().PadLeft(4, '0'),
                    @"\",
                    aggregate.CreationDate.Month.ToString().PadLeft(2, '0'),
                    @"\",
                    aggregate.CreationDate.Day.ToString().PadLeft(2, '0'),
                    @"\",
                    aggregate.CreationDate.Hour.ToString().PadLeft(2, '0'),
                    @"\",
                    Guid.NewGuid().ToString(),
                    @"\",
                    aggregate.FileName);

            this._logger.LogDebug($"GenerateId: {id}");

            aggregate.AssignId(id);
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            idTenant = idTenant ?? throw new ArgumentNullException(nameof(idTenant));
            id = id ?? throw new ArgumentNullException(nameof(id));

            id = id.PathAsUnixPath();

            this._logger.LogDebug($"AsUnixPath: {id} - Exists: {File.Exists(id)}");

            return File.Exists(id);
        }

        protected override async Task<DocumentBlob> HandleGet(DocumentBlob aggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            idTenant = idTenant ?? throw new ArgumentNullException(nameof(idTenant));
            id = id ?? throw new ArgumentNullException(nameof(id));

            id = id.PathAsUnixPath();

            this._logger.LogDebug($"AsUnixPath: {id} - Exists: {File.Exists(id)}");

            if (!File.Exists(id))
                throw new DocumentBlobNotFoundPi3Exception(id);

            var fileInfo = new FileInfo(id);

            var events = new List<IEvent>();

            events.Add(new ElementCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                CreationDate = fileInfo.CreationTime,
                Name = new TextValue(fileInfo.Name)
            });

            events.Add(new FileNameLoadedEvent()
            {
                FileName = fileInfo.Name
            });

            events.Add(new StreamLoadedEvent()
            {
                Stream = new BufferedStream(new FileStream(id, FileMode.Open, FileAccess.Read), _defaultBufferSize)
            });

            this.LoadAggregateFromHistory(aggregate, events.ToArray());

            return aggregate;
        }

        protected override async Task HandleUpdate(DocumentBlob aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleDelete(DocumentBlob aggregate)
        {
            aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            var id = aggregate.Id.PathAsUnixPath();

            this._logger.LogDebug($"AsUnixPath: {id} - Exists: {File.Exists(id)}");

            if (!File.Exists(id))
                throw new DocumentBlobNotFoundPi3Exception(id);

            File.Delete(id);
        }

        protected override async Task HandleAdd(DocumentBlob aggregate)
        {
            aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            var uncommitted = new List<dynamic>(aggregate.GetUncommittedChanges());

            foreach (var @event in uncommitted)
            {
                var handleMethod = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "Handle"
                        && m.GetParameters().Any(p => p.ParameterType == @event.GetType()));

                if (handleMethod != null)
                    await this.Handle(@event, aggregate);
            }
        }

        protected virtual async Task Handle(StreamUploadedEvent @event, DocumentBlob aggregate)
        {
            await this.GenerateId(aggregate);

            await this.AssertFileAlreadyExists(aggregate);

            var id = aggregate.Id.PathAsUnixPath();

            this._logger.LogDebug($"id: {id}");

            var directoryName = Path.GetDirectoryName(id);

            this._logger.LogDebug($"directoryName: {directoryName}");

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            var lastPosition = @event.Stream.Position;

            @event.Stream.Position = 0;

            try
            {
                using (var stream = new FileStream(id, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    using (var bufferedStream = new BufferedStream(stream, _defaultBufferSize))
                    {
                        int readCount;
                        byte[] buffer = new byte[_defaultBufferSize];
                        while ((readCount = @event.Stream.Read(buffer, 0, _defaultBufferSize)) != 0)
                            bufferedStream.Write(buffer, 0, readCount);

                        await bufferedStream.FlushAsync();
                    }
                }

                this._logger.LogDebug($"Scrittura file '{id}' completata.");
            }
            catch
            {
                throw;
            }
            finally
            {
                @event.Stream.Position = lastPosition;
            }
        }

        protected virtual async Task Handle(PathUploadedEvent @event, DocumentBlob aggregate)
        {
            await this.GenerateId(aggregate);

            await this.AssertFileAlreadyExists(aggregate);

            var id = aggregate.Id.PathAsUnixPath();

            this._logger.LogDebug($"id: {id}");

            var directoryName = Path.GetDirectoryName(id);

            this._logger.LogDebug($"directoryName: {directoryName}");

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            File.Copy(@event.Path, id);

            this._logger.LogDebug($"Scrittura file '{id}' completata.");

            aggregate.LoadStream(new BufferedStream(new FileStream(id, FileMode.Open, FileAccess.Read), _defaultBufferSize));

            aggregate.MarkChangesAsCommitted();
        }

        #endregion
    }
}
