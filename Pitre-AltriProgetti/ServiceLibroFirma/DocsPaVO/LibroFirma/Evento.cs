using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class Evento
    {
        #region private fields

        private string _idEvento;
        private string _codiceAzione;
        private string _tipoEvento;
        private string _idDocumento;
        private string _idRuolo;
        private string _idUtente;
        private string _idProcesso;
        private string _dataInserimento;
        private string _delegato;

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
        /// Id documento correlato all'evento
        /// </summary>
        public string idDocumento
        {
            get
            {
                return _idDocumento;
            }

            set
            {
                _idDocumento = value;
            }
        }

        /// <summary>
        /// Id documento correlato all'evento
        /// </summary>
        public string idRuolo
        {
            get
            {
                return _idRuolo;
            }

            set
            {
                _idRuolo = value;
            }
        }

        /// <summary>
        /// Id documento correlato all'evento
        /// </summary>
        public string idUtente
        {
            get
            {
                return _idUtente;
            }

            set
            {
                _idUtente = value;
            }
        }

        /// <summary>
        /// Id documento correlato all'evento
        /// </summary>
        public string idProcesso
        {
            get
            {
                return _idProcesso;
            }

            set
            {
                _idProcesso = value;
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

        /// <summary>
        /// Data inserimento log
        /// </summary>
        public string DataInserimento
        {
            get { return _dataInserimento; }
            set { _dataInserimento = value; }
        }

        /// <summary>
        /// People id del delegato
        /// </summary>
        public string Delegato
        {
            get
            {
                return _delegato;
            }

            set
            {
                _delegato = value;
            }
        }
        #endregion
    }
}
