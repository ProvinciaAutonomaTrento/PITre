// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari.Add
{
    [ResourceSwaggerSchema("DestinatarioAdd_Head")]
    public class Destinatario
    {
        [ResourceSwaggerSchema("DestinatarioAdd_Codice")]
        public string Codice { get; set; }
    }
}
