// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Spreadsheet
{
    public class CellModel 
    {
        [Required]
        [Range(0, Int32.MaxValue)]
        public int Column { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int Row { get; set; }

        public string? Name { get; init; } = null;

        public string? ValueAsString { get; set; } = null;

        public string? DataType { get; set; } = null;

        public CellStyleModel? CellStyle { get; set; } = null;

        public virtual bool TryParse<T>(out T? value)
        {
            if (this.ValueAsString == null)
            {
                value = default(T?);
                return false;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));

                try
                {
                    value = (T?)converter.ConvertFromString(this.ValueAsString);
                    return true;
                }
                catch
                {
                    value = default(T?);
                    return false;
                }
            }
        }
    }
}
