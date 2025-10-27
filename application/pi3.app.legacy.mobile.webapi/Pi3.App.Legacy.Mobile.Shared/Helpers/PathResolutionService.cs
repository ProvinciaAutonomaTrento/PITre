// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Pi3.App.Legacy.Mobile.Shared.Helpers;

public class PathResolutionService( IActionDescriptorCollectionProvider actionDescriptorCollectionProvider )
{
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;

    public string? ResolvePath( string controllerName, string actionName )
    {
        var actionDescriptor = this._actionDescriptorCollectionProvider.ActionDescriptors.Items
            .FirstOrDefault(ad => ad.RouteValues["Controller"] == controllerName
                               && ad.RouteValues["Action"] == actionName);

        return actionDescriptor?.AttributeRouteInfo?.Template;
    }
}
