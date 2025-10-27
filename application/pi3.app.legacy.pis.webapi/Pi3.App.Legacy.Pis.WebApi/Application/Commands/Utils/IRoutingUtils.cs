// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils
{
    public interface IRoutingUtils
    {
        string[] GetEndpointAndApps(string administrationCode);

        string GetEndpointAndInstance(string codeAdm, string AppName, out string instance);

    }
}
