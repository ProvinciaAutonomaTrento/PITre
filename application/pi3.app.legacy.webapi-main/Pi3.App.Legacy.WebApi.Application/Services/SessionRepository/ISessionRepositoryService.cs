// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.SessionRepository
{
    public interface ISessionRepositoryService : IService
    {
        Task<DocsPaVO.documento.SessionRepositoryContext> CreateRepository(DocsPaVO.utente.InfoUtente infoUtente);

        Task<bool> FileExists(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest);

        Task SetFile(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento file);

        Task<DocsPaVO.documento.FileDocumento> GetFile(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest);

        Task RemoveFile(DocsPaVO.documento.SessionRepositoryContext context, DocsPaVO.documento.FileRequest fileRequest);

        Task RemoveAllFiles(DocsPaVO.documento.SessionRepositoryContext context);

        Task Syncronize(DocsPaVO.documento.SchedaDocumento schedaDocumento);

        Task DeleteRepository(DocsPaVO.documento.SessionRepositoryContext context);
    }
}
