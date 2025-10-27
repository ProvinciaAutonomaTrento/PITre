// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Qualifica
{
    /// <summary>
    /// Rappresenta un utente con una particolare qualifica
    /// </summary>
    [Serializable()]
    public class PeopleQualifica
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdPeopleGroups
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Matricola
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string NomeCompleto
        {
            get;
            set;
        }
    }
}
