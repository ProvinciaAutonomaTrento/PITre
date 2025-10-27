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
    public class SpreadsheetModel
    {
        public SpreadsheetModel()
        {
            this._sheets = new List<SheetModel>();
        }

        public void AddSheet(SheetModel sheetModel)
        {
            sheetModel = sheetModel ?? throw new ArgumentNullException(nameof(sheetModel));
            Validator.ValidateObject(sheetModel, new ValidationContext(sheetModel));

            this._sheets.Add(sheetModel);
        }

        public IReadOnlyList<SheetModel> Sheets
        {
            get
            {
                return this._sheets.AsReadOnly();
            }
        }

        public List<SheetModel> _sheets = null!;
    }
}
