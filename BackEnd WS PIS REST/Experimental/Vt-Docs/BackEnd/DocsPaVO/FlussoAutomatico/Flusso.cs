using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.FlussoAutomatico
{
    public class InfoDocumentoFlusso
    {
        private string idProfile;
        private string numProto;
        private string oggetto;
        private string nomeRegistroIn = string.Empty;
        private string numeroRegistroIn = string.Empty;
        private string dataRegistroIn = string.Empty;

        /// <summary>
        /// Id del documento
        /// </summary>
        public string ID_PROFILE
        {
            get
            {
                return idProfile;
            }
            set
            {
                idProfile = value;
            }
        }

        /// <summary>
        /// Numero di protocollo
        /// </summary>
        public string NUM_PROTO
        {
            get
            {
                return numProto;
            }
            set
            {
                numProto = value;
            }
        }

        /// <summary>
        /// Oggetto del doucmento
        /// </summary>
        public string OGGETTO
        {
            get
            {
                return oggetto;
            }
            set
            {
                oggetto = value;
            }
        }

        /// <summary>
        /// Nome del registro 
        /// </summary>
        public string NOME_REGISTRO_IN
        {
            get
            {
                return nomeRegistroIn;
            }
            set
            {
                nomeRegistroIn = value;
            }
        }

        /// <summary>
        /// Numero del registro
        /// </summary>
        public string NUMERO_REGISTRO_IN
        {
            get
            {
                return numeroRegistroIn;
            }
            set
            {
                numeroRegistroIn = value;
            }
        }

        /// <summary>
        /// Data registro
        /// </summary>
        public string DATA_REGISTRO_IN
        {
            get
            {
                return dataRegistroIn;
            }
            set
            {
                dataRegistroIn = value;
            }
        }
    }

    public class Flusso
    {
        private string systemId;
        private string idProcesso;
        private Messaggio messaggio;
        private string dataArrivo;
        private InfoDocumentoFlusso infoDocumento;

        /// <summary>
        /// Id del flusso
        /// </summary>
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

        /// <summary>
        /// Identificatore di processo
        /// </summary>
        public string ID_PROCESSO
        {
            get
            {
                return idProcesso;
            }
            set
            {
                idProcesso = value;
            }
        }

        /// <summary>
        /// Tipologia di messaggio
        /// </summary>
        public Messaggio MESSAGGIO
        {
            get
            {
                return messaggio;
            }
            set
            {
                messaggio = value;
            }
        }

        /// <summary>
        /// Data di arrivo del messaggio
        /// </summary>
        public string DATA_ARRIVO
        {
            get
            {
                return dataArrivo;
            }
            set
            {
                dataArrivo = value;
            }
        }

        /// <summary>
        /// Info del documento
        /// </summary>
        public InfoDocumentoFlusso INFO_DOCUMENTO
        {
            get
            {
                return infoDocumento;
            }
            set
            {
                infoDocumento = value;
            }
        }
    }
}
