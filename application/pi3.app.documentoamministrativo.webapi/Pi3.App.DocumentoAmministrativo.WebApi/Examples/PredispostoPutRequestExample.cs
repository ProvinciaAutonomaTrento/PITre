// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Swashbuckle.AspNetCore.Filters;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Examples
{
    internal class PredispostoPutRequestExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return Resources.Examples.PredisponiProtocolloTest_Actual;
        }
    }

}
