// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Deleghe;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses.Deleghe;
public class GetDelegheResponse
{
    public IEnumerable<Delega>? Elements { get; set; }
    public int TotalRecordCount { get; set; }
}
