// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.Models.ServiceRequests;
public class GetDocumentRequest
{
    public long IdDocumento { get; set; }
    public long? IdRuoloPubblico { get; set; }
}
