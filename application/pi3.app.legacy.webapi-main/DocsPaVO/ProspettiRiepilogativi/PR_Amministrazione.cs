// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.ProspettiRiepilogativi
{
    /// <summary>
    /// Summary description for Amministrazione.
    /// </summary>
    public class PR_Amministrazione
    {
        #region oggetto PR_Amministrazione
        private string _codice;
        private string _descrizione;
        private string _libreria;
        private string _system_id;


        public PR_Amministrazione()
        {

        }

        public PR_Amministrazione(string system_id, string codice, string descrizione, string libreria)
        {
            _system_id = system_id;
            _codice = codice;
            _descrizione = descrizione;
            _libreria = libreria;
        }

        #region proprietà

        public string System_id
        {
            get
            {
                return _system_id;
            }
            set
            {
                _system_id = value;
            }
        }

        public string Codice
        {
            get
            {
                return _codice;
            }
            set
            {
                _codice = value;
            }
        }

        public string Descrizione
        {
            get
            {
                return _descrizione;
            }
            set
            {
                _descrizione = value;
            }
        }

        public string Libreria
        {
            get
            {
                return _libreria;
            }
            set
            {
                _libreria = value;
            }
        }



        #endregion


    }
    #endregion

    #region oggetto registro



    #endregion


}
