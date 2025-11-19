// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Refit;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability
{
    public class InteroperabilityMockService : IInteroperabilityService
    {
        public Task AnalyzeDocumentDroppedOrErrorMessageProof(string instance, [Header("Authorization")] string authorization, [Header("tenant")] string tenant, [Body] AnalyzeDocumentDroppedOrErrorMessageProofRequest request)
        {
            throw new NotImplementedException();
        }

        public Task AnalyzeDocumentReceivedProof(string instance, [Header("Authorization")] string authorization, [Header("tenant")] string tenant, [Body] AnalyzeDocumentReceivedProofRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ElaborateNewInteroperabilityMessageResponse> ElaborateNewInteroperabilityMessage(string instance, [Header("Authorization")] string authorization, [Header("tenant")] string tenant, [Body] ElaborateNewInteroperabilityMessageRequest request)
        {
            return new ElaborateNewInteroperabilityMessageResponse()
            {
                Result = new ElaborateInteroperabilityMessageResult()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    DocumentDelivered = new InfoDocumentDelivered()
                    { 
                        MainDocument = request.InteroperabilityMessage.MainDocument,
                        Attachments = request.InteroperabilityMessage.Attachments
                    }
                }
            };
        }
    }
}
