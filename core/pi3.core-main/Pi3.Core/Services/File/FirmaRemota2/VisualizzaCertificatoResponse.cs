// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public class VisualizzaCertificatoResponse : ValueObject
    {
        public DateTime DataVerifica { get; set; }
        public List<Esito> Esito { get; set; } = null!;
    }
}
