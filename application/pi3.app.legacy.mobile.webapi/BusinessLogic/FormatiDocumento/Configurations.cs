// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace BusinessLogic.FormatiDocumento
{
    /// <summary>
    /// Configurazioni per la gestione formato documenti
    /// </summary>
    public sealed class Configurations
    {
        /// <summary>
        /// Constante, identifica la chiave di configurazione per abilitare / disabilitare la funzione
        /// </summary>
        private const string SUPPORTED_FILE_TYPES_ENABLED = "SUPPORTED_FILE_TYPES_ENABLED";

        /// <summary>
        /// 
        /// </summary>
        private Configurations()
        { }

        /// <summary>
        /// Abilitazione / disabilitazione funzione
        /// </summary>
        public static bool SupportedFileTypesEnabled
        {
            get
            {
                bool supported;
                bool.TryParse(DocsPaVO.Settings.AppSettings.Instance.SUPPORTED_FILE_TYPES_ENABLED, out supported);
                return supported;
            }
        }
    }
}