// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FileValidator
{
    public class FileValidationCompliance
    {
        public bool IsCompliantToFormat { get; set; }

        public bool? IsExecutable { get; set; } = false;

        public bool? HasMacro { get; set; } = false;
        public bool? HasForms { get; set; } = false;
        public bool? HasJavascript { get; set; } = false;
    }

    public class FileValidationResult
    {
        public bool NameIsValid { get; set; }

        public bool FormatIsAdmitted { get; set; }

        public FileValidationCompliance? Compliance { get; set; } = null;
    }
}
