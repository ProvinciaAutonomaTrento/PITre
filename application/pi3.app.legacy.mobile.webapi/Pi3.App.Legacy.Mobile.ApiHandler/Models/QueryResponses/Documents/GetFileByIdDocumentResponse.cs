// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses.Documents;
public class GetFileByIdDocumentResponse
{
    public DTO.Documents.File? File { get; set; }

    public GetFileDocumentByIdResponseCode Code { get; set; } = GetFileDocumentByIdResponseCode.OK;
    public string? ErrorMessage { get; set; }

    public enum GetFileDocumentByIdResponseCode { OK, SYSTEM_ERROR }
}
