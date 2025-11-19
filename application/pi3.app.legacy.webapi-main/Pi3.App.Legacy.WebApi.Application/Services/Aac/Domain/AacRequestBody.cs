// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.Aac.Domain
{
    public class AacRequestBody
    {
        [AliasAs("grant_type")]
        public string GrantType { get; set; }
        [AliasAs("client_id")]
        public string ClientId { get; set; }
        [AliasAs("client_secret")]
        public string ClientSecret { get; set; }
        [AliasAs("scope")]
        public string Scope { get; set; }
    }
}
