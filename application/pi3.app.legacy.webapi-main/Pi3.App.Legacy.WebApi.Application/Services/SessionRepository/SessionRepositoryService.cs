// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.SessionRepository
{
    public class SessionRepositoryService : ISessionRepositoryService
    {        
        #region Public Members

        public SessionRepositoryService(
            ILogger<SessionRepositoryService> logger, 
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._configurationService = configurationService;
        }

        public async Task<DocsPaVO.documento.SessionRepositoryContext> CreateRepository(DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                var context = new DocsPaVO.documento.SessionRepositoryContext(infoUtente);

                string path = await this.GetContextRepositoryPath(context);

                // Creazione directory del repository
                Directory.CreateDirectory(path);

                // Serializzazione owner 
                System.IO.File.WriteAllText(Path.Combine(path, "owner.dat"), JsonConvert.SerializeObject(infoUtente));

                return context;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task Syncronize(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            try
            {
                if (schedaDocumento.repositoryContext == null)
                    throw new ApplicationException("Nessun repository di sessione definito per il documento");

                await this.ValidateRepository(schedaDocumento.repositoryContext);

                // Imposta il repository come scaduto
                await this.DeleteRepository(schedaDocumento.repositoryContext);

                schedaDocumento.repositoryContext = null!;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task DeleteRepository(DocsPaVO.documento.SessionRepositoryContext context)
        {
            try
            {
                await this.ValidateRepository(context);

                var path = await this.GetContextRepositoryPath(context);

                foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
                    file.Delete();
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> FileExists(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {
                var path = await this.GetFilePath(context, fileRequest);

                await this.ValidateRepository(context);

                return System.IO.File.Exists(path);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task<FileDocumento> GetFile(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {
                await this.ValidateRepository(context);

                var path = await this.GetFilePath(context, fileRequest);

                var fileDocumentContent = System.IO.File.ReadAllText(path);

                return JsonConvert.DeserializeObject<DocsPaVO.documento.FileDocumento>(fileDocumentContent)!;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task RemoveAllFiles(DocsPaVO.documento.SessionRepositoryContext context)
        {
            try
            {
                await this.ValidateRepository(context);

                var path = await this.GetContextRepositoryPath(context);

                foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
                    file.Delete();
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task RemoveFile(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {
                await this.ValidateRepository(context);
                
                var path = await this.GetFilePath(context, fileRequest);

                System.IO.File.Delete(path);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        public async Task SetFile(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest, FileDocumento file)
        {
            try
            {
                var path = await this.GetFilePath(context, fileRequest);

                await this.ValidateRepository(context);

                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(file));
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, ex.Message);

                throw;
            }
        }

        #endregion

        #region Private Members

        protected readonly IConfigurationService _configurationService;
        protected readonly ILogger<SessionRepositoryService> _logger;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        protected async Task<string> GetContextRepositoryPath(DocsPaVO.documento.SessionRepositoryContext context)
        {
            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

            var rootPath = Path.Combine(repositoryRootPath.PathAsUnixPath(), "SessionRepository");
            this._logger.LogInformation("GetContextRepositoryPath: " + rootPath);

            return Path.Combine(rootPath, context.Token);
        }

        protected virtual async Task<string> GetFilePath(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest)
        {
            if (fileRequest is DocsPaVO.documento.Allegato)
                return await this.GetAttatchmentFilePath(context, (DocsPaVO.documento.Allegato)fileRequest);
            else
            {
                var path = await this.GetContextRepositoryPath(context);

                return Path.Combine(path, fileRequest.version);
            }
        }

        protected async Task<string> GetAttatchmentFilePath(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.Allegato attatchment)
        {
            string path = await this.GetOrCreateAttatchmentFolder(context, attatchment);

            return Path.Combine(path, attatchment.version);
        }

        protected async Task<string> GetOrCreateAttatchmentFolder(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.Allegato attatchment)
        {
            var path = await GetContextRepositoryPath(context);

            return Directory.CreateDirectory(Path.Combine(path, attatchment.versionLabel)).FullName;
        }

        protected async Task ValidateRepository(DocsPaVO.documento.SessionRepositoryContext context)
        {
            var path = await this.GetContextRepositoryPath(context);

            if (!Directory.Exists(path))
                throw new ApplicationException(string.Format("Il repository '{0}' risulta scaduto e non può essere più utilizzato", context.Token));

            // Reperimento dell'owner persistito  e confronto con l'owner corrente
            // per verificare utilizzi non autorizzati
            var fileContent = System.IO.File.ReadAllText(Path.Combine(path, "owner.dat"));

            var owner = JsonConvert.DeserializeObject<DocsPaVO.utente.InfoUtente>(fileContent);

            if (!owner.dst.Equals(context.Owner.dst))
                throw new ApplicationException(string.Format("Utente {0} non autorizzato all'utilizzo del repository", context.Owner.userId));
        }

        #endregion
    }
}
