// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Keywords.Aggiungi
{
    [ResourceSwaggerSchema("AggiungiKeyWord_Head")]
    public class AggiungiKeyWord
    {
        [ResourceSwaggerSchema("AggiungiKeyWord_Valore")]
        public string Valore { get; set; }
    }
}
