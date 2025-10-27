// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using SERVICE_REQUEST = Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;

namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IDocumentService
{
    Task<SERVICE_DTO.Documento> GetDocumentAsync(
        SERVICE_REQUEST.GetDocumentRequest request,
        CancellationToken cancellationToken );

    Task<IEnumerable<SERVICE_DTO.Documento>> GetDocumentAttachmentsAsync(
        SERVICE_REQUEST.GetDocumentRequest request, 
        CancellationToken cancellationToken );

    Task<SERVICE_DTO.FileInfo> GetFileByIdDocument( 
        long idDocumento, 
        CancellationToken cancellationToken = default );
}
