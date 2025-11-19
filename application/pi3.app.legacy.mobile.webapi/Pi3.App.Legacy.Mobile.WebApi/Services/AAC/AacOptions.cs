// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.WebApi.Services.AAC
{
    public class AacOptions
    {
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string BaseUrl { get; set; }
    }
}
