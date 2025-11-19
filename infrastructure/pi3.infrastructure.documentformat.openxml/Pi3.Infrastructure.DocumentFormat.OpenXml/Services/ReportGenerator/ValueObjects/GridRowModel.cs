// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.DocumentFormatOpenXml.Services.ReportGenerator.ValueObjects
{
    public class GridRowModel 
    {
        public GridRowModel()
        {
            this._cells = new List<GridCellModel>();
        }

        public void AddCell(GridCellModel cell)
        {
            cell = cell ?? throw new ArgumentNullException(nameof(cell));
            Validator.ValidateObject(cell, new ValidationContext(cell));

            this._cells.Add(cell);
        }

        public IReadOnlyList<GridCellModel> Cells
        {
            get
            {
                return this._cells.AsReadOnly();
            }
        }

        private readonly List<GridCellModel> _cells = null!;
    }

}
