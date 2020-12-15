using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public enum TipoStatoElemento
    {
        PROPOSTO,
        DA_FIRMARE,
        DA_RESPINGERE,
        FIRMATO,
        RESPINTO,
        INTERROTTO,
        NO_COMPETENZA
    }

    public class ElementoInLibroFirma
    {
        #region private fields

        private string _idElemento;

        private TipoStatoElemento _statoFirma;
        private string _tipoFirma;
        private string _modalita;

        private string _dataInserimento;
        private string _dataScadenza;
        private DocsPaVO.utente.Ruolo _ruoloProponente;
        private DocsPaVO.utente.Utente _utenteProponente;
        private string _idRuoloTitolare;
        private string _idUtenteTitolare;
        private string _idUtenteLocker;

        private string _note;

        private InfoDocLibroFirma _infoDocumento;

        private string _idIstanzaProcesso;
        private string _motivoRespingimento;
        private string _idIstanzaPasso;
        private string _idTrasmSingola;
        private string _dataAccettazione;
        private string _erroreFirma;
        private string _descrizioneProponenteDelegato;
        private string _fileOriginaleFirmato;
        private string _tipoFirmaFile;

        private long _fileSize;
        #endregion

        #region public property



        public string IdElemento
        {
            get { return _idElemento; }
            set { _idElemento = value; }
        }

        public TipoStatoElemento StatoFirma
        {
            get { return _statoFirma; }
            set { _statoFirma = value; }
        }

        public string TipoFirma
        {
            get { return _tipoFirma; }
            set { _tipoFirma = value; }
        }

        public string Modalita
        {
            get { return _modalita; }
            set { _modalita = value; }
        }

        public string DataInserimento
        {
            get { return _dataInserimento; }
            set { _dataInserimento = value; }
        }

        public string DataScadenza
        {
            get { return _dataScadenza; }
            set { _dataScadenza = value; }
        }

        public DocsPaVO.utente.Ruolo RuoloProponente
        {
            get { return _ruoloProponente; }
            set { _ruoloProponente = value; }
        }

        public DocsPaVO.utente.Utente UtenteProponente
        {
            get { return _utenteProponente; }
            set { _utenteProponente = value; }
        }

        public string IdRuoloTitolare
        {
            get { return _idRuoloTitolare; }
            set { _idRuoloTitolare = value; }
        }

        public string IdUtenteTitolare
        {
            get { return _idUtenteTitolare; }
            set { _idUtenteTitolare = value; }
        }

        public string IdUtenteLocker
        {
            get { return _idUtenteLocker; }
            set { _idUtenteLocker = value; }
        }

        public InfoDocLibroFirma InfoDocumento
        {
            get { return _infoDocumento; }
            set { _infoDocumento = value; }
        }

        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        public string IdIstanzaProcesso
        {
            get { return _idIstanzaProcesso; }
            set { _idIstanzaProcesso = value; }
        }

        public string MotivoRespingimento
        {
            get { return _motivoRespingimento; }
            set { _motivoRespingimento = value; }
        }

        public string IdIstanzaPasso
        {
            get { return _idIstanzaPasso; }
            set { _idIstanzaPasso = value; }
        }

        public string IdTrasmSingola
        {
            get
            {
                return _idTrasmSingola;
            }
            set
            {
                _idTrasmSingola = value;
            }
        }

        public string DataAccettazione
        {
            get
            {
                return _dataAccettazione;
            }
            set
            {
                _dataAccettazione = value;
            }
        }

        public long FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
            }
        }

        public string ErroreFirma
        {
            get
            {
                return _erroreFirma;
            }
            set
            {
                _erroreFirma = value;
            }
        }

        public string DescProponenteDelegato
        {
            get
            {
                return _descrizioneProponenteDelegato;
            }
            set
            {
                _descrizioneProponenteDelegato = value;
            }
        }

        public string FileOriginaleFirmato
        {
            get
            {
                return _fileOriginaleFirmato;
            }
            set
            {
                _fileOriginaleFirmato = value;
            }
        }

        public string TipoFirmaFile
        {
            get
            {
                return _tipoFirmaFile;
            }
            set
            {
                _tipoFirmaFile = value;
            }
        }
        #endregion
    }
}
