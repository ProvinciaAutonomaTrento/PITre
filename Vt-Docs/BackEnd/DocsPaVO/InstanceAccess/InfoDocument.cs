using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess
{
    public class InfoDocument
    {
        #region private fields

        private string _docnumber;
        private string _object;
        private string _hash;
        private string _fileName;
        private string _typeProto;
        private string _numberProto;
        private string _mittDest;
        private string _register;
        private string _descriptionTipologiaAtto;
        private string _counterRepertory;
        private string _classification;
        private string _codeProject;
        private string _descriptionProject;
        private string _idDocumentoPrincipale;
        private int _typeAttach;
        private string _extension;
        private DateTime _dateCreation;
        private bool _isSigned;

        #endregion

        #region public property

        public string DOCNUMBER
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

        public string OBJECT
        {
            get
            {
                return _object;
            }

            set
            {
                _object = value;
            }
        }

        public string HASH
        {
            get
            {
                return _hash;
            }

            set
            {
                _hash = value;
            }
        }

        public string FILE_NAME
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }

        public string TYPE_PROTO
        {
            get
            {
                return _typeProto;
            }

            set
            {
                _typeProto = value;
            }
        }

        public string NUMBER_PROTO
        {
            get
            {
                return _numberProto;
            }

            set
            {
                _numberProto = value;
            }
        }

        public string MITT_DEST
        {
            get
            {
                return _mittDest;
            }

            set
            {
                _mittDest = value;
            }
        }

        public string REGISTER
        {
            get
            {
                return _register;
            }

            set
            {
                _register = value;
            }
        }

        public string DESCRIPTION_TIPOLOGIA_ATTO
        {
            get
            {
                return _descriptionTipologiaAtto;
            }

            set
            {
                _descriptionTipologiaAtto = value;
            }
        }

        public string COUNTER_REPERTORY
        {
            get
            {
                return _counterRepertory;
            }

            set
            {
                _counterRepertory = value;
            }
        }

        public string CLASSIFICATION
        {
            get
            {
                return _classification;
            }

            set
            {
                _classification = value;
            }
        }

        public string CODE_PROJECT
        {
            get
            {
                return _codeProject;
            }

            set
            {
                _codeProject = value;
            }
        }

        public string DESCRIPTION_PROJECT
        {
            get
            {
                return _descriptionProject;
            }

            set
            {
                _descriptionProject = value;
            }
        }

        /// <summary>
        /// Valorizzato nel caso in cui il documento è un allegato
        /// </summary>
        public string ID_DOCUMENTO_PRINCIPALE
        {
            get
            {
                return _idDocumentoPrincipale;
            }

            set
            {
                _idDocumentoPrincipale = value;
            }
        }

        public int TYPE_ATTACH
        {
            get
            {
                return _typeAttach;
            }

            set
            {
                _typeAttach = value;
            }
        }

        public string EXTENSION
        {
            get
            {
                return _extension;
            }

            set
            {
                _extension = value;
            }
        }

        public DateTime DATE_CREATION
        {
            get
            {
                return _dateCreation;
            }

            set
            {
                _dateCreation = value;
            }
        }

        public bool IS_SIGNED
        {
            get
            {
                return _isSigned;
            }

            set
            {
                _isSigned = value;
            }
        }
        
        #endregion
    }
}
