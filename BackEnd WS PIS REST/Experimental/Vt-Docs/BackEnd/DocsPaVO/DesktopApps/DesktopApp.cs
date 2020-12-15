using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.DesktopApps
{
    public class DesktopApp
    {

        #region private fields

        private string _Nome;
        private string _Versione;
        private string _Path;
        private string _Descrizione;

        #endregion

        #region public property

        /// <summary>
        /// Nome dell'applicazione desktop
        /// </summary>
        public string Nome
        {
            get
            {
                return _Nome;
            }

            set
            {
                _Nome = value;
            }
        }

        /// <summary>
        /// Versione attuale
        /// </summary>
        public string Versione
        {
            get
            {
                return _Versione;
            }

            set
            {
                _Versione = value;
            }
        }

        /// <summary>
        /// Path di prelevamento dell'applicazione desktop
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                _Path = value;
            }
        }

        /// <summary>
        /// Descrizione
        /// </summary>
        public string Descrizione
        {
            get
            {
                return _Descrizione;
            }

            set
            {
                _Descrizione = value;
            }
        }

        #endregion
    }
}
