// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using Pi3.App.Legacy.Mobile.Shared.Helpers;
using System.Text;

namespace Pi3.App.Legacy.Mobile.WebApi.Tests;
public class TestApplication
{
    readonly HttpClient _apiHttpClient = MobileTestContext.Instance.ApiHttpClient;
    readonly PathResolutionService? _pathResolutionService = MobileTestContext.Instance.PathResolutionService;

    //[Test(Description = "Get Lista Istanze")]
    public async Task GetListaIstanze()
    {
        string? url = this._pathResolutionService!.ResolvePath("AppSettings", "GetInstanceList");

        HttpRequestMessage? request = new(HttpMethod.Get, url);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Assert.Fail("Test fallito");
        }
    }

}
