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
        private string _ruoloProponente;
        private string _utenteProponente;
        private string _idRuoloTitolare;
        private string _idUtenteTitolare;
        private string _idUtenteLocker;
        
        private string _note;

        private InfoDocLibroFirma _infoDocumento;
        private string _idIstanzaProcesso;

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

        public string IdRuoloProponente
        {
            get { return _ruoloProponente; }
            set { _ruoloProponente = value; }
        }

        public string IdUtenteProponente
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
        #endregion
    }
}
