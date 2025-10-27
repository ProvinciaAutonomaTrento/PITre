// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentReceivedProof;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.SendProof;
using Refit;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Mock
{
    public class MockSendProofService : IInteroperabilityService
    {
        public async Task AnalyzeDocumentReceivedProof(string instance, [Authorize("Bearer")] string token, [Header("tenant")] string tenant, [Body] AnalyzeDocumentReceivedProofRequest request)
        {
            return;
        }
    }
}
