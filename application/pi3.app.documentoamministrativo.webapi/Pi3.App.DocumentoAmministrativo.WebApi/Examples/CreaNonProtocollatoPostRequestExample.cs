// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Swashbuckle.AspNetCore.Filters;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Examples
{
    internal class CreaNonProtocollatoPostRequestExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return Resources.Examples.CreaNonProtocollatoTest_Actual;
        }
    }

}
