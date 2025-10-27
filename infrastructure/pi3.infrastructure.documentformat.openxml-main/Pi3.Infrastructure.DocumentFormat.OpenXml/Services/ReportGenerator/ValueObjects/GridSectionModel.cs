// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.DocumentFormatOpenXml.Services.ReportGenerator.ValueObjects
{
    public class GridSectionModel : ISectionModel
    {
        public GridSectionModel()
        {
            this._rows = new List<GridRowModel>();
        }

        public GridSectionStyleModel? Style { get; set; } = null!;

        public void AddRow(GridRowModel row)
        {
            row = row ?? throw new ArgumentNullException(nameof(row));
            Validator.ValidateObject(row, new ValidationContext(row));

            this._rows.Add(row);
        }
        
        public IReadOnlyList<GridRowModel> Rows
        {
            get
            {
                return this._rows.AsReadOnly();
            }
        }

        private readonly List<GridRowModel> _rows = null!;
    }

}
