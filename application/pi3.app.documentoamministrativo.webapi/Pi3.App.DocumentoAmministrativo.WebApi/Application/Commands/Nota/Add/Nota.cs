// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Add
{
    [ResourceSwaggerSchema("NuovaNota_Head")]
    public class Nota
    {
        [ResourceSwaggerSchema("Nota_Nome")]
        public string nome { get; set; } = "";

        [ResourceSwaggerSchema("Nota_Descrizione")]
        public string description { get; set; } = "";
        [ResourceSwaggerSchema("NuovaNota_TipoAccesso")]
        public TipoAccessoNotaEnum tipoAccesso { get; set; }
        public string? idAccessoRF { get; set; }
    }
}
