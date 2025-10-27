// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails
{
    public class GetEmailsResponse
    {
        public IReadOnlyList<Email>? Emails { get; set; } = null;
    }
}
