using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{
    public class DescrizioneFascicolo
    {
        private string systemId;
        private string descrizione;
        private string codice;
        private string idRegistro;
        private string codRegistro;
        private string idAmm;

        public string SystemId
        {
            get { return systemId; }
            set { systemId = value; }
        }

        public string Descrizione
        {
            get { return descrizione; }
            set { descrizione = value; }
        }

        public string Codice
        {
            get { return codice; }
            set { codice = value; }
        }

        public string IdRegistro
        {
            get { return idRegistro; }
            set { idRegistro = value; }
        }

        public string CodRegistro
        {
            get { return codRegistro; }
            set { codRegistro = value; }
        }

        public string IdAmm
        {
            get { return idAmm; }
            set { idAmm = value; }
        }
    }

    [XmlInclude(typeof(DocsPaVO.fascicolazione.listaDescrizioniFascicoli))]
    public class FiltroDescrizioniFascicolo
    {
        #region private fields

        private string _argomento;
        private string _valore;

        #endregion

        #region Public property

        public DocsPaVO.fascicolazione.listaDescrizioniFascicoli listaDescrizioniFascicoli;

        public string Argomento
        {
            get
            {
                return _argomento;
            }
            set
            {
                _argomento = value;
            }
        }

        public string Valore
        {
            get
            {
                return _valore;
            }
            set
            {
                _valore = value;
            }
        }
        #endregion
    }

    [XmlType("FiltriDescrizioniFascicoli")]
    public enum listaDescrizioniFascicoli
    {
        CODICE,
        DESCRIZIONE,
        REGISTRO
    }

    public enum ResultDescrizioniFascicolo
    {
        OK,
        DESCRIZIONE_PRESENTE,
        KO
    }
}
