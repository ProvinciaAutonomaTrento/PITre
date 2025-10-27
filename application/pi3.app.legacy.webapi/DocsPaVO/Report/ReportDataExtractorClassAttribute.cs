// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Attributo da utilizzare per la decorazione di classi che contengono metodi per la
    /// generazione di reportistica
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ReportDataExtractorClassAttribute : Attribute
    {
    }
}
