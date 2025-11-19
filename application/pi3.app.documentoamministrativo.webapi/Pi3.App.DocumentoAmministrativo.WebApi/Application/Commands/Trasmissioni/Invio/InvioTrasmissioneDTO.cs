// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Invio
{
    [ResourceSwaggerSchema("InvioTrasmissioneDTO")]
    public class InvioTrasmissioneDTO
    {
        [ResourceSwaggerSchema("DataInvio")]
        public DateTime DataInvio { get; set; }
    }
}
