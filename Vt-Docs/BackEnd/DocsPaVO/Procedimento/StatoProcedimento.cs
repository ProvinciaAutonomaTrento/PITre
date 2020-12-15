using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Procedimento
{
    [Serializable]
    public class StatoProcedimento
    {
        private String _idStato;
        private String _descStato;
        private String _idFase;
        private String _descFase;
        private bool _statoIniziale;
        private bool _statoFinale;
        private DateTime _dataStato;

        public String IdStato
        {
            get
            {
                return _idStato;
            }
            set
            {
                _idStato = value;
            }
        }

        public String IdFase
        {
            get
            {
                return _idFase;
            }
            set
            {
                _idFase = value;
            }
        }

        public String DescrizioneStato
        {
            get
            {
                return _descStato;
            }
            set
            {
                _descStato = value;
            }
        }

        public String DescrizioneFase
        {
            get
            {
                return _descFase;
            }
            set
            {
                _descFase = value;
            }
        }

        public DateTime DataStato
        {
            get
            {
                return _dataStato;
            }
            set
            {
                _dataStato = value;
            }
        }

        public bool StatoIniziale
        {
            get
            {
                return _statoIniziale;
            }
            set
            {
                _statoIniziale = value;
            }
        }

        public bool StatoFinale
        {
            get
            {
                return _statoFinale;
            }
            set
            {
                _statoFinale = value;
            }
        }
    }
}
