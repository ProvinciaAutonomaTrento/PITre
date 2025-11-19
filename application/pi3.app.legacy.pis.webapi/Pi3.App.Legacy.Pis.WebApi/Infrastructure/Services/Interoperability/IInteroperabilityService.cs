// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Refit;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability
{
    public interface IInteroperabilityService
    {

        [Put("/{instance}/InteroperabilityService/AnalyzeDocumentDroppedOrErrorMessageProof")]
        Task AnalyzeDocumentDroppedOrErrorMessageProof(
            string instance,
            [Header("Authorization")] string authorization,
            [Header("tenant")] string tenant,
            [Body] AnalyzeDocumentDroppedOrErrorMessageProofRequest request);

        [Put("/{instance}/InteroperabilityService/AnalyzeDocumentReceivedProof")]
        Task AnalyzeDocumentReceivedProof(
            string instance,
            [Header("Authorization")] string authorization,
            [Header("tenant")] string tenant,
            [Body] AnalyzeDocumentReceivedProofRequest request);

        [Post("/{instance}/InteroperabilityService/ElaborateNewInteroperabilityMessage")]
        Task<ElaborateNewInteroperabilityMessageResponse> ElaborateNewInteroperabilityMessage(
            string instance,
            [Header("Authorization")] string authorization,
            [Header("tenant")] string tenant,
            [Body] ElaborateNewInteroperabilityMessageRequest request);
    }
}
