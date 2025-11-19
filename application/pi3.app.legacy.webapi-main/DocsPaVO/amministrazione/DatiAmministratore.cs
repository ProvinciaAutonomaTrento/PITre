// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using DocsPaVO.utente;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Oggetto contenente i dati di connessione relativi all'utente amministratore
    /// </summary>
    public class InfoUtenteAmministratore : InfoUtente
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoUtenteAmministratore()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ut"></param>
        /// <param name="ruo"></param>
        public InfoUtenteAmministratore(Utente ut, Ruolo ruo)
            : base(ut, ruo)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string nome = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string cognome = string.Empty;

        /// <summary>
        /// Tipo di amministratore
        /// </summary>
        public string tipoAmministratore = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.Menu))]
        public DocsPaVO.amministrazione.Menu[] VociMenu { get; set; }
    }
}
