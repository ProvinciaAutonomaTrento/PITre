// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.SeedWork;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Cryptography;
using System.Net.Mime;
using System.IO;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Pi3.Core.AggregateModels.DocumentBlobAggregate
{
    public enum HashNamesEnum
    {
        SHA256,
        SHA512
    }

    public class DocumentBlob : ContentElement
    {
        #region Public Members

        protected DocumentBlob() : base()
        { }

        public DocumentBlob(string id, string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(id, idTenant, "DocumentBlob", creationDate, name, description)
        {
        }

        public DocumentBlob(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, "DocumentBlob", creationDate, name, description)
        {
        }

        public override IEnumerable<Pi3Exception> GetErrors()
        {
            var errors = new List<Pi3Exception>();

            if (string.IsNullOrWhiteSpace(this.FileName))
                errors.Add(new DocumentBlobPi3Exception(ErrorDescriptions.MissingFileName, ErrorDescriptions.ResourceManager, this.Id));

            if (!this.GetUncommittedChanges().Any(c => c.GetType() == typeof(PathUploadedEvent) || c.GetType() == typeof(StreamLoadedEvent)))
                errors.Add(new DocumentBlobPi3Exception(ErrorDescriptions.MissingFileUploaded, ErrorDescriptions.ResourceManager, this.Id));

            return errors;
        }

        public virtual void LoadFileName(string fileName, string? contentType = null)
        {
            fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

            this.AssertValidFileName(fileName);

            this.ApplyChange(new FileNameLoadedEvent()
            {
                Id = this.Id,
                FileName = fileName,
                ContentType = contentType
            });
        }

        public virtual void LoadStream(Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            this.ApplyChange(new StreamLoadedEvent()
            {
                Id = this.Id,
                Stream = stream
            });
        }

        public virtual void UploadStream(Stream stream, string fileName, string? contentType = null)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

            this.AssertValidFileName(fileName);

            this.AssertFileAlreadyUploaded();

            this.ApplyChange(new StreamUploadedEvent()
            {
                Id = this.Id,
                Stream = stream,
                FileName = fileName,
                ContentType = contentType
            });
        }

        public virtual void UploadFromPath(string path, string? fileName = null, string? contentType = null)
        {
            path = path ?? throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundPi3Exception(path);

            this.AssertValidFileName(fileName ?? Path.GetFileName(path));

            this.AssertFileAlreadyUploaded();

            this.ApplyChange(new PathUploadedEvent()
            {
                Id = this.Id,
                Path = path,
                FileName = fileName,
                ContentType = contentType
            });
        }

        public virtual void ComputeHash(HashNamesEnum hashName, int bufferSize = 1048576)
        {
            var stream = this.Stream;
            var disposeStream = false;

            if (stream == null && !string.IsNullOrWhiteSpace(this._path))
            {
                stream = File.OpenRead(this._path);
                disposeStream = true;
            }

            if (stream == null)
                throw new DocumentBlobPi3Exception(ErrorDescriptions.FileContentNotLoaded, ErrorDescriptions.ResourceManager, this.Id);

            try
            {
                using (var bufferedStream = new BufferedStream(stream, bufferSize))
                {
                    using (var algorithm = System.Security.Cryptography.HashAlgorithm.Create(hashName.ToString()))
                    {
                        byte[] hash = algorithm.ComputeHash(bufferedStream);

                        this.ApplyChange(new HashComputedEvent()
                        {
                            Id = this.Id,
                            Hash = hash, 
                            HashName = hashName
                        });
                    }
                }
            }
            catch
            { }
            finally
            {
                if (disposeStream)
                    stream.Dispose();
            }
        }

        public string FileName { get; protected set; }

        public string ContentType { get; protected set; }

        public long? FileSize { get; protected set; }

        public Stream Stream { get; protected set; }

        public byte[]? Hash { get; protected set; }

        public HashNamesEnum? HashName { get; protected set; }

        #endregion

        #region Private Members

        protected string _path = null;

        protected virtual void AssertValidFileName(string fileName)
        {
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                throw new DocumentBlobPi3Exception(ErrorDescriptions.InvalidFileName, ErrorDescriptions.ResourceManager, this.Id);
        }

        protected virtual void AssertFileAlreadyUploaded()
        {
            if (this.GetUncommittedChanges().Any(c => c.GetType() == typeof(PathUploadedEvent) || c.GetType() == typeof(StreamLoadedEvent)))
                throw new DocumentBlobPi3Exception(ErrorDescriptions.FileAlreadyUploaded, ErrorDescriptions.ResourceManager, this.Id);
        }

        protected virtual void Handle(FileNameLoadedEvent @event)
        {
            this.FileName = @event.FileName;
            this.ContentType = @event.ContentType ?? this.DiscoverMimeType(@event.FileName);
        }

        protected virtual void Handle(StreamLoadedEvent @event)
        {
            this._path = null;
            this.Stream = @event.Stream;
            this.FileSize = @event.Stream.Length;
            this.Hash = null;
            this.HashName = null;
        }

        protected virtual void Handle(StreamUploadedEvent @event)
        {
            this.FileName = @event.FileName;
            this.ContentType = @event.ContentType ?? this.DiscoverMimeType(this.FileName);
            this.Stream = @event.Stream;
            this.FileSize = @event.Stream.Length;
            this.Hash = null;
            this.HashName = null;
        }

        protected virtual void Handle(PathUploadedEvent @event)
        {
            this._path = @event.Path;

            var fi = new FileInfo(this._path);

            this.FileName = @event.FileName ?? fi.Name;
            this.ContentType = @event.ContentType ?? this.DiscoverMimeType(this.FileName);
            this.Stream = null;
            this.FileSize = fi.Length;
            this.Hash = null;
            this.HashName = null;
        }

        protected virtual void Handle(HashComputedEvent @event)
        {
            this.Hash = @event.Hash;
            this.HashName = @event.HashName;
        }

        protected virtual string DiscoverMimeType(string fileName)
        {
            FileExtensionContentTypeProvider d = new FileExtensionContentTypeProvider();
            
            string mimeType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out mimeType);
            return mimeType ?? "application/octet-stream";
        }
        
        #endregion
    }
}