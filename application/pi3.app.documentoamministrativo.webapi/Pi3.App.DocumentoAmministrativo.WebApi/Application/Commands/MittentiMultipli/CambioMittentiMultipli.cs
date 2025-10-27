// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.MittentiMultipli;

[ResourceSwaggerSchema("CambioMittentiMultipli_Head")]
public class CambioMittentiMultipli
{
    [ResourceSwaggerSchema("CambioMittentiMultipli_Mittente")]
    public string Mittente { get; init; }
}
