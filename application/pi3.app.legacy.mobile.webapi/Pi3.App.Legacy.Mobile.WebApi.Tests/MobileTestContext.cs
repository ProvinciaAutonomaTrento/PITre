// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.WebApi.Tests;
internal class MobileTestContext
{
    #region Singleton pattern
    private static MobileTestContext? instance;

    public static MobileTestContext Instance { 
        get
        {
            instance ??= new MobileTestContext();
            return instance;
        }
    }

    #endregion

    #region Context variables
    public string? AuthenticationToken { get; set; }

    public HttpClient ApiHttpClient { get; set; } = null!;

    public PathResolutionService PathResolutionService { get; set; } = null!;

    public dynamic TestData { get; set; } = null!;

    #endregion
}
