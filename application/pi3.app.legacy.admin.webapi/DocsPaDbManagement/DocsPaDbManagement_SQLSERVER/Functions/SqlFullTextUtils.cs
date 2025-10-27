// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;
using DocsPaUtils.Data;

namespace DocsPaDbManagement.Functions
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlFullTextUtils : FullTextUtils
    {
         /// <summary>
        /// 
        /// </summary>
        public SqlFullTextUtils()
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public override string ParseTextSpecialChars(string queryString)
        {
            return queryString;
        }
    }
}
