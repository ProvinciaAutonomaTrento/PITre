using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.FlussoAutomatico
{
    public class ContestoProcedurale
    {
        private string systemId;
        private string tipoContestoProcedurale;
        private string nome;
        private string famiglia;
        private string versione;

        public string SYSTEM_ID
        {
            get
            {
                return systemId;
            }
            set
            {
                systemId = value;
            }
        }

        public string TIPO_CONTESTO_PROCEDURALE
        {
            get
            {
                return tipoContestoProcedurale;
            }
            set
            {
                tipoContestoProcedurale = value;
            }
        }

        public string NOME
        {
            get
            {
                return nome;
            }
            set
            {
                nome = value;
            }
        }

        public string FAMIGLIA
        {
            get
            {
                return famiglia;
            }
            set
            {
                famiglia = value;
            }
        }

        public string VERSIONE
        {
            get
            {
                return versione;
            }
            set
            {
                versione = value;
            }
        }
    }
}
