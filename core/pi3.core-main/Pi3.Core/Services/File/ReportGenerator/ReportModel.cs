// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class ReportModel 
    {
        public ReportModel()
        {
            this._headerSections = new List<ISectionModel>();
            this._sections = new List<ISectionModel>();
            this._footerSections = new List<ISectionModel>();
        }

        public ReportOutputTypes OutputType { get; set; } = ReportOutputTypes.AsDocx;

        public PageSizes Size { get; set; } = PageSizes.A4;

        public PageOrientations Orientation { get; set; } = PageOrientations.Portrait;

        public void AddHeaderSection(ISectionModel section)
        {
            section = section ?? throw new ArgumentNullException(nameof(section));
            Validator.ValidateObject(section, new ValidationContext(section));

            this._headerSections.Add(section);
        }

        public void AddSection(ISectionModel section)
        {
            section = section ?? throw new ArgumentNullException(nameof(section));
            Validator.ValidateObject(section, new ValidationContext(section));

            this._sections.Add(section);
        }

        public void AddFooterSection(ISectionModel section)
        {
            section = section ?? throw new ArgumentNullException(nameof(section));
            Validator.ValidateObject(section, new ValidationContext(section));

            this._footerSections.Add(section);
        }

        public IReadOnlyList<ISectionModel> HeaderSections
        {
            get
            {
                return this._headerSections.AsReadOnly();
            }
        }

        public IReadOnlyList<ISectionModel> Sections
        {
            get
            {
                return this._sections.AsReadOnly();
            }
        }

        public IReadOnlyList<ISectionModel> FooterSections
        {
            get
            {
                return this._footerSections.AsReadOnly();
            }
        }

        private readonly List<ISectionModel> _headerSections = null!;
        private readonly List<ISectionModel> _sections = null!;
        private readonly List<ISectionModel> _footerSections = null!;
    }

}
