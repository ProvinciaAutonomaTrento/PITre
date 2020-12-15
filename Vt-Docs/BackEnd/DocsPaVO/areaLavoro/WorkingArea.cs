using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.areaLavoro
{
    public class WorkingArea
    {
        private string _idObject;
        private TipoOggetto _objectType;
        private string _tipoDocumento;
        private string _tipoFascicolo;
        private string _idRegistro;
        private string _dataInserimento;
        private string _motivo;
        private string _idPeople;
        private string _idRuolo;

        /// <summary>
        /// Id Oggetto in area lavoro
        /// </summary>
        public string IdObject
        {
            get
            {
                return _idObject;
            }

            set
            {
                _idObject = value;
            }
        }

        /// <summary>
        /// Tipo oggetto
        /// </summary>
        public TipoOggetto ObjectType
        {
            get
            {
                return _objectType;
            }

            set
            {
                _objectType = value;
            }
        }

        /// <summary>
        /// Tipo documento
        /// </summary>
        public string TipoDocumento
        {
            get
            {
                return _tipoDocumento;
            }

            set
            {
                _tipoDocumento = value;
            }
        }

        /// <summary>
        /// Tipo fascicolo
        /// </summary>
        public string TipoFascicolo
        {
            get
            {
                return _tipoFascicolo;
            }

            set
            {
                _tipoFascicolo = value;
            }
        }

        /// <summary>
        /// Id registro
        /// </summary>
        public string IdRegistro
        {
            get
            {
                return _idRegistro;
            }

            set
            {
                _idRegistro = value;
            }
        }

        /// <summary>
        /// Data inserimento in area di lavoro
        /// </summary>
        public string DataInserimento
        {
            get
            {
                return _dataInserimento;
            }

            set
            {
                _dataInserimento = value;
            }
        }

        /// <summary>
        /// Motivo inserimento in area lavoro
        /// </summary>
        public string Motivo
        {
            get
            {
                return _motivo;
            }

            set
            {
                _motivo = value;
            }
        }

        /// <summary>
        /// Id people
        /// </summary>
        public string IdPeople
        {
            get
            {
                return _idPeople;
            }

            set
            {
                _idPeople = value;
            }
        }

        /// <summary>
        /// Id ruolo
        /// </summary>
        public string IdRuolo
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
    }
}
