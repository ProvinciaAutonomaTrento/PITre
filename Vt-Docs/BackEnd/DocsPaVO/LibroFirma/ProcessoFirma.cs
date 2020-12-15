using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.LibroFirma
{
    public class ProcessoFirma
    {
        #region private fields

        private string _idProcesso;
        private string _nome;
        private string _idRuoloAutore;
        private string _idPeopleAutore;
        private List<PassoFirma> _passi;
        private bool _isInvalidated;
        private bool _isProcessModel;
        private string _dataCreazione;
        private string _idStatoInterruzione;

        #endregion

        #region public property

        /// <summary>
        /// System id del processo di firma
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
        /// Nome del processo di firma
        /// </summary>
        public string nome
        {
            get
            {
                return _nome;
            }

            set
            {
                _nome = value;
            }
        }

        /// <summary>
        /// Id del ruolo autore del processo di firma
        /// </summary>
        public string idRuoloAutore
        {
            get
            {
                return _idRuoloAutore;
            }

            set
            {
                _idRuoloAutore = value;
            }
        }

        /// <summary>
        /// Id dell'uUtente autore del processo di firma
        /// </summary>
        public string idPeopleAutore
        {
            get
            {
                return _idPeopleAutore;
            }

            set
            {
                _idPeopleAutore = value;
            }
        }

        /// <summary>
        /// Passi che compongono il processo di firma
        /// </summary>
        public List<PassoFirma> passi
        {
            get
            {
                return _passi;
            }
            set
            {
                _passi = value;
            }
        }

        /// <summary>
        /// Se true il processo è invalidato
        /// </summary>
        public bool isInvalidated
        {
            get
            {
                return _isInvalidated;
            }
            set
            {
                _isInvalidated = value;
            }
        }

        /// <summary>
        /// Se true è un modello di processo
        /// </summary>
        public bool IsProcessModel
        {
            get
            {
                return _isProcessModel;
            }
            set
            {
                _isProcessModel = value;
            }
        }

        /// <summary>
        /// Data di creazione del processo
        /// </summary>
        public string DataCreazione
        {
            get
            {
                return _dataCreazione;
            }

            set
            {
                _dataCreazione = value;
            }
        }

        /// <summary>
        /// Id stato del diagramma in cui sposare il documento in caso di interruzione del processo
        /// </summary>
        public string IdStatoInterruzione
        {
            get
            {
                return _idStatoInterruzione;
            }
            set
            {
                _idStatoInterruzione = value;
            }
        }
        #endregion
    }

    public enum ResultProcessoFirma
    {
        /// <summary>
        /// successo
        /// </summary>
        OK,
        DOCUMENTO_GIA_IN_LIBRO_FIRMA,
        DOCUMENTO_CONSOLIDATO,
        DOCUMENTO_BLOCCATO,
        EXISTING_PROCESS_NAME,
        FILE_NON_AMMESSO_ALLA_FIRMA,
        FILE_NON_ACQUISITO,
        KO,
        PASSO_PROTO_DOC_GIA_PROTOCOLLATO,
        PASSO_PROTO_DOC_NON_PREDISPOSTO,
        PASSO_REP_DOC_GIA_REPERTORIATO,
        PASSO_REP_DOC_NON_TIPIZZATO,
        PASSO_REP_NESSUN_CONTATORE_TIPO_DOC,
        PASSO_REP_NO_DIRITTI_SCRITTURA_CONTATORE,
        PASSO_REP_RF_MANCANTE,
        PASSO_SPEDIZIONE_PROTO_ARRIVO,
        PASSO_PADES_SU_FILE_CADES,
        PROCESSO_ATTIVO_PER_DOC_PRINCIPALE,
        PASSO_SPEDIZIONE_DOC_NON_PROTOCOLLATO,
        PASSO_AUTOMATICO_REGISTRO_ERRATO,
        PASSO_PROTO_REG_CHIUSO,
        PASSO_FACOLTATIVO_INCLUDI_NON_SPECIFICATO,
        PASSO_FACOLTATIVO_NON_INCLUSO_UNICO_PASSO_PROCESSO,
        RUOLO_NON_ABILITATO_A_CREAZIONE_MODELLI,
        RUOLO_NON_ABILITATO_A_CREAZIONE_PASSO_PROTO_AUTO,
        RUOLO_NON_ABILITATO_A_CREAZIONE_PASSO_SPEDIZIONE_AUTO,
        RUOLO_NON_ABILITATO_A_CREAZIONE_PASSO_REPO_AUTO
    }

    public enum TipoVisibilita
    {
         MONITORATORE = 'M',
         PROPONENTE = 'P',
         PROPONENTE_MONITORATORE = 'T'
    }

    public class VisibilitaProcessoRuolo
    {
        private string idProcesso;
        private DocsPaVO.utente.Ruolo ruolo;
        private TipoVisibilita tipoVisibilita;
        private OpzioniNotifica notifica;

        public string IdProcesso
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

        public DocsPaVO.utente.Ruolo Ruolo
        {
            get
            {
                return ruolo;
            }

            set
            {
                ruolo = value;
            }
        }

        public TipoVisibilita TipoVisibilita
        {
            get
            {
                return tipoVisibilita;
            }

            set
            {
                tipoVisibilita = value;
            }
        }

        public OpzioniNotifica Notifica
        {
            get
            {
                return notifica;
            }
            set
            {
                notifica = value;
            }
        }
    }

    public class CopiaProcessiFirmaResult
    {
        public string idProcesso;
        public string nomeProcesso;
        public ResultProcessoFirma esito;
    }
}
