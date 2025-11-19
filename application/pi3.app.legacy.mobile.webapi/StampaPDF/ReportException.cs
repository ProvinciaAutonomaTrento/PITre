// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace StampaPDF
{
    /// <summary>
    /// Summary description for ReportException.
    /// </summary>
    public class ReportException : Exception
    {
        public ErrorCode code = ErrorCode.GenericError;

        public ReportException(ErrorCode code, string description) : base(description)
        {
            this.code = code;
        }

        public ReportException()
        {
        }
    }

}
