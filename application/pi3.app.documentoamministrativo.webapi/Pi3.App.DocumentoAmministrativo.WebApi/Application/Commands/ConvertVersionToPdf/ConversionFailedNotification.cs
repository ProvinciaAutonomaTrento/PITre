// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.ConvertVersionToPdf
{
    public class ConversionFailedNotification : INotification
    {
        [Required(AllowEmptyStrings = false)]
        public string IdRequest { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string IdTenant { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string IdDocument { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string IdUser { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string IdGroup { get; set; } = null!;

        [Required]
        public DateTime ProcessingDate { get; set; }

        [Required]
        public double ProcessingElapsed { get; set; }

        [Required]
        public string Error { get; set; } = null!;
    }
}
