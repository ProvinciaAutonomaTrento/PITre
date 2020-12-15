using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class AnagraficaEventi
    {
        #region private fields

        private string _idEvento;
        private string _codiceAzione;
        private string _descrizione;
        private string _tipoEvento;

        #endregion

        #region public property

        /// <summary>
        /// System id dell'evento
        /// </summary>
        public string idEvento
        {
            get
            {
                return _idEvento;
            }

            set
            {
                _idEvento = value;
            }
        }

        /// <summary>
        /// Codice azione dell'evento
        /// </summary>
        public string codiceAzione
        {
            get
            {
                return _codiceAzione;
            }

            set
            {
                _codiceAzione = value;
            }
        }

        /// <summary>
        /// Descrizione dell'evento
        /// </summary>
        public string descrizione
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

        /// <summary>
        /// Descrizione dell'evento
        /// </summary>
        public string tipoEvento
        {
            get
            {
                return _tipoEvento;
            }

            set
            {
                _tipoEvento = value;
            }
        }
        #endregion
    }
}
