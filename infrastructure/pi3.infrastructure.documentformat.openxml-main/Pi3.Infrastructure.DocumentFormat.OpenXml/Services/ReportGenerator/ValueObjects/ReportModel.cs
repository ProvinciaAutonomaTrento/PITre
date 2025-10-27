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
    public class ReportModel 
    {
        public ReportModel()
        {
            this._sections = new List<ISectionModel>();
        }

        public PageSizes Size { get; set; } = PageSizes.A4;

        public PageOrientations Orientation { get; set; } = PageOrientations.Portrait;

        public void AddSection(ISectionModel section)
        {
            section = section ?? throw new ArgumentNullException(nameof(section));
            Validator.ValidateObject(section, new ValidationContext(section));

            this._sections.Add(section);
        }

        public IReadOnlyList<ISectionModel> Sections
        {
            get
            {
                return this._sections.AsReadOnly();
            }
        }

        private readonly List<ISectionModel> _sections = null!;
    }

}
