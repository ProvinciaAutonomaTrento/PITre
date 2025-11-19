// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Gestione dati relativi alle configurazioni delle password per l'amministrazione
    /// </summary>
    [Serializable()]
    public class PasswordConfigurations
    {
        /// <summary>
        /// 
        /// </summary>
        public PasswordConfigurations()
        {
        }

        public int IdAmministrazione = 0;
        public bool ExpirationEnabled = false;
        public int ValidityDays = 0;
        public int MinLength = 0;
        public char[] SpecialCharacters = new char[0];
    }
}
