// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.LibroFirma
{
    [XmlType("StatoPasso")]
    public class StatoPasso
    {
        /// <summary>
        /// Non eseguito
        /// </summary>
        public static int NEW = 1;
        /// <summary>
        /// In attesa
        /// </summary>
        public static int LOOK = 2;
        /// <summary>
        /// Concluso
        /// </summary>
        public static int CLOSE = 3;
        /// <summary>
        /// Interrotto
        /// </summary>
        public static int STUCK = 4;
    }
}
