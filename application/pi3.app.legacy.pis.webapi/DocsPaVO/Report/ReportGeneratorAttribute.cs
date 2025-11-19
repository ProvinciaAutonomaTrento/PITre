// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Report
{

    /// <summary>
    /// Questo attributo viene utilizzato per decorare le classi responsabile della generazione di un report
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ReportGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Nome del report
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Nome del contesto
        /// </summary>
        public String ContextName { get; set; }

        /// <summary>
        /// Chiave identificativa del report
        /// </summary>
        public String Key { get; set; }

    }

}
