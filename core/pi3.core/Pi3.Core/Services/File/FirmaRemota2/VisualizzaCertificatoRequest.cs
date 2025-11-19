// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public class VisualizzaCertificatoRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string AliasCertificato { get; init; } = null!;

        public string DominioCertificato { get; init; } = null!;

        public string UserPwd { get; init; } = null!;    
    }
}
