// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Spreadsheet
{
    public class SheetModel 
    {
        public SheetModel()
        {
            this._cells = new List<CellModel>();
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;

        public void AddCell(CellModel cellModel)
        {
            cellModel = cellModel ?? throw new ArgumentNullException(nameof(cellModel));
            Validator.ValidateObject(cellModel, new ValidationContext(cellModel));

            this._cells.Add(cellModel);
        }

        public IReadOnlyList<CellModel> Cells
        {
            get
            {
                return this._cells.AsReadOnly();
            }
        }

        protected List<CellModel> _cells = null!;
    }
}
