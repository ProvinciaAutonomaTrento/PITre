using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento
{
    public class Procedimento
    {
        private String _id;
        private String _idEsterno;
        private String _autore;
        private String _descrizione;
        private EsitoProcedimento _esito = EsitoProcedimento.InCorso;
        private DocumentoProcedimento[] _documenti;

        public String Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public String IdEsterno
        {
            get
            {
                return _idEsterno;
            }
            set
            {
                _idEsterno = value;
            }
        }

        public String Descrizione
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

        public String Autore
        {
            get
            {
                return _autore;
            }
            set
            {
                _autore = value;
            }
        }

        public EsitoProcedimento Esito
        {
            get
            {
                return _esito;
            }
            set
            {
                _esito = value;
            }
        }

        public DocumentoProcedimento[] Documenti
        {
            get
            {
                return _documenti;
            }
            set
            {
                _documenti = value;
            }
        }

    }

    public enum EsitoProcedimento
    {
        Positivo = 1,
        Negativo = -1,
        InCorso = 0
    }
}
