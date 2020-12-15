using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess
{
    public class InstanceAccess
    {
        #region private fields

        private string _idInstanceAccess;
        private string _description;
        private DateTime _creationDate;
        private DateTime _closeDate;
        private string _idPeopleOwner;
        private string _idGroupsOwner;
        private DocsPaVO.utente.Corrispondente _richiedente;
        private DateTime _requestDate;
        private string _idDocumentRequest;
        private string _note;
        private char _stateDownloadForward;
        private List<InstanceAccessDocument> _documents;


        #endregion

        #region public property

        /// <summary>
        /// System id dell'istanza si accesso
        /// </summary>
        public string ID_INSTANCE_ACCESS
        {
            get
            {
                return _idInstanceAccess;
            }

            set
            {
                _idInstanceAccess = value;
            }
        }

        /// <summary>
        /// Descrizione dell'istanza di accesso
        /// </summary>
        public string DESCRIPTION
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Data di creazione dell'istanza di accesso
        /// </summary>
        public DateTime CREATION_DATE
        {
            get
            {
                return _creationDate;
            }

            set
            {
                _creationDate = value;
            }
        }

        /// <summary>
        /// Data di chiusura dell'istanza di accesso
        /// </summary>
        public DateTime CLOSE_DATE
        {
            get
            {
                return _closeDate;
            }

            set
            {
                _closeDate = value;
            }
        }

        /// <summary>
        /// Id people dell'utente proprietario dell'istanza di accesso
        /// </summary>
        public string ID_PEOPLE_OWNER
        {
            get
            {
                return _idPeopleOwner;
            }

            set
            {
                _idPeopleOwner = value;
            }
        }

        /// <summary>
        /// Id gruppo dell'utente proprietario dell'istanza di accesso
        /// </summary>
        public string ID_GROUPS_OWNER
        {
            get
            {
                return _idGroupsOwner;
            }

            set
            {
                _idGroupsOwner = value;
            }
        }

        /// <summary>
        /// System id dell'utente richiedente l'istanza di accesso
        /// </summary>
        public DocsPaVO.utente.Corrispondente RICHIEDENTE
        {
            get
            {
                return _richiedente;
            }

            set
            {
                _richiedente = value;
            }
        }


        /// <summary>
        /// Data in cui è stata richiesta l'istanza di accesso
        /// </summary>
        public DateTime REQUEST_DATE
        {
            get
            {
                return _requestDate;
            }

            set
            {
                _requestDate = value;
            }
        }

        /// <summary>
        /// Id del documento per cui è stata richiesta l'istanza di accesso
        /// </summary>
        public string ID_DOCUMENT_REQUEST
        {
            get
            {
                return _idDocumentRequest;
            }

            set
            {
                _idDocumentRequest = value;
            }
        }

        /// <summary>
        /// Nota dell'istanza di accesso
        /// </summary>
        public string NOTE
        {
            get
            {
                return _note;
            }

            set
            {
                _note = value;
            }
        }

        /// <summary>
        /// Stato del download del contenuto dell'istanza di accesso
        /// </summary>
        public char STATE_DOWNLOAD_FORWARD
        {
            get
            {
                return _stateDownloadForward;
            }

            set
            {
                _stateDownloadForward = value;
            }
        }

        /// <summary>
        /// Documenti apparteneti all'istanza
        /// </summary>
        public List<InstanceAccessDocument> DOCUMENTS
        {
            get
            {
                return _documents;
            }

            set
            {
                _documents = value;
            }

        }

        #endregion

    }
}
