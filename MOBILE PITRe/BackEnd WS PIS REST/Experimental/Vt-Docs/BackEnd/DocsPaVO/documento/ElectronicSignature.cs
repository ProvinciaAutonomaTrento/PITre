using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    public class ElectronicSignature
    {
        #region private fields

        private string _idElectronicSignature;
        private string _docnumber;
        private string _versionId;
        private string _dateAffixing;
        private string _ruoloFirmatario; 
        private string _utenteFirmatario;
        private string _xmlElectronicSignature;

        #endregion

        #region public property

        public string IdElectronicSignature
        {
            get
            {
                return _idElectronicSignature;
            }

            set
            {
                _idElectronicSignature = value;
            }
        }

        /// <summary>
        /// System id del documento
        /// </summary>
        public string Docnumber
        {
            get
            {
                return _docnumber;
            }

            set
            {
                _docnumber = value;
            }
        }

        /// <summary>
        /// Version Id del documento
        /// </summary>
        public string VersionId
        {
            get
            {
                return _versionId;
            }
            set
            {
                _versionId = value;
            }
        }

        /// <summary>
        /// Data apposizione firma elettronica
        /// </summary>
        public string DateAffixing
        {
            get
            {
                return _dateAffixing;
            }
            set
            {
                _dateAffixing = value;
            }
        }

        /// <summary>
        /// Ruolo firmatario
        /// </summary>
        public string RuoloFirmatario
        {
            get
            {
                return _ruoloFirmatario;
            }
            set
            {
                _ruoloFirmatario = value;
            }
        }

        /// <summary>
        /// Utente firmatario
        /// </summary>
        public string UtenteFirmatario
        {
            get
            {
                return _utenteFirmatario;
            }
            set
            {
                _utenteFirmatario = value;
            }
        }

        /// <summary>
        /// XML della firma elletronica
        /// </summary>
        public string XmlElectronicSignature
        {
            get
            {
                return _xmlElectronicSignature;
            }
            set
            {
                _xmlElectronicSignature = value;
            }
        }
        #endregion
    }
}
