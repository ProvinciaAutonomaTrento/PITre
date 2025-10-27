// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Annullamento
{
    [ResourceSwaggerSchema("Annullamento_Head")]
    public class Annullamento
    {
        [ResourceSwaggerSchema("Annullamento_IdAutore")]
        public string IdAutore { get; set; }

        [ResourceSwaggerSchema("Annullamento_Motivo")]
        public string Motivo { get; set; }

        [ResourceSwaggerSchema("Annullamento_Data")]
        public DateTime Data { get; set; }
    }
}
