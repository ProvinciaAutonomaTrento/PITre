// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Http;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.Interoperability
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
