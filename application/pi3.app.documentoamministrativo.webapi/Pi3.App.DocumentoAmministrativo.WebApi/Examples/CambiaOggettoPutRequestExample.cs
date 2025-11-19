// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Swashbuckle.AspNetCore.Filters;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Examples
{
    internal class CambiaOggettoPutRequestExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return Resources.Examples.CambiaOggettoTest_Actual;
        }
    }

}
