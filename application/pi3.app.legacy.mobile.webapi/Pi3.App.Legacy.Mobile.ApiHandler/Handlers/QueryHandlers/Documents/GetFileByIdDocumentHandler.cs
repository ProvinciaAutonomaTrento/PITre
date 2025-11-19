// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Documents;
public class GetFileByIdDocumentHandler(
    IDocumentService documentService,
    IMapper mapper
    ) : IRequestHandler<QUERY.GetFileByIdDocumentRequest, RESPONSE.Documents.GetFileByIdDocumentResponse>
{
    private readonly IDocumentService _documentService = documentService;
    private readonly IMapper _mapper = mapper;

    public async Task<RESPONSE.Documents.GetFileByIdDocumentResponse> Handle( QUERY.GetFileByIdDocumentRequest request, CancellationToken cancellationToken )
    {
        SERVICE_DTO.FileInfo file = await this._documentService.GetFileByIdDocument(request.Id, cancellationToken);

        RESPONSE.Documents.GetFileByIdDocumentResponse response = new ()
        {
            File = this._mapper.Map<DTO.Documents.File>(file)
        };


        return response;
    }
}
