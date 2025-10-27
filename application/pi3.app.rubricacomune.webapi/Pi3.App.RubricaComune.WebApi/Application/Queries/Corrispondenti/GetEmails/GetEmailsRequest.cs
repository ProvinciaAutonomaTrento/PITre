// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails
{
    public class GetEmailsRequest : IRequest<GetEmailsResponse>
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; } = null!;
    }
}
