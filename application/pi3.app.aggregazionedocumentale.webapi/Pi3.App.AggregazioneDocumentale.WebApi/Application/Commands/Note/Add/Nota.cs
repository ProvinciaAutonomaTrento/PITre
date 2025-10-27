// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Note.Add
{
    [ResourceSwaggerSchema(nameof(Documentation.NotaAdd))]
    public class Nota
    {
        [ResourceSwaggerSchema(nameof(Documentation.Nota_Nome))]
        public string nome { get; set; } = "";
        [ResourceSwaggerSchema(nameof(Documentation.Nota_Description))]
        public string description { get; set; } = "";
        [ResourceSwaggerSchema(nameof(Documentation.Nota_Autore))]
        public AutoreNota? autore { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Nota_TipoAccesso))]
        public TipoAccessoNotaEnum tipoAccesso { get; set; }
        public string? idAccessoRF { get; set; }
    }
}
