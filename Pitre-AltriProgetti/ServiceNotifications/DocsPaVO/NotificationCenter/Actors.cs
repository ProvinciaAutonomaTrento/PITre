using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    /// <summary>
    /// Questa classe contiene le informazioni sul produttore di un evento
    /// </summary>
    public class Producer
    {
        #region private fields

        private string _descProducer;
        private int _idRole;
        private int _idUser;
        private int _idUserDelegator;

        #endregion

        #region public property


        /// <summary>
        /// descrizione utente produttore dell'evento
        /// </summary>
        public string DES_PRODUCER
        {
            set
            {
                _descProducer = value;
            }
            get
            {
                return _descProducer;
            }
        }

        /// <summary>
        /// Producer user
        /// </summary>
        public int IDUSER
        {
            get
            {
                return _idUser;
            }
            set
            {
                _idUser = value;
            }
        }

        /// <summary>
        /// Role of the producer
        /// </summary>
        public int IDROLE
        {
            get
            {
                return _idRole;
            }
            set
            {
                _idRole = value;
            }
        }

        /// <summary>
        /// Id utente delegante
        /// </summary>
        public int IDUSERDELEGATOR
        {
            get
            {
                return _idUserDelegator;
            }
            set
            {
                _idUserDelegator = value;
            }
        }
        #endregion
    }


    /// <summary> 
    /// Questa classe contiene la lista dei destinatari della notifica associata all'evento
    /// </summary>
    public class Recipient
    {
        #region private fields

        private int _idRole;
        private int _idUser;
        private char _operationalORinformation;

        #endregion

        #region constructor

        /// <summary>
        /// Inizializza il destinatario
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="idUser"></param>
        public Recipient(int idRole, int idUser)
        {
            this._idRole = idRole;
            this._idUser = idUser;
            this._operationalORinformation = SupportStructures.NotificationType.OPERATIONAL;
        }

        /// <summary>
        /// costruttore per destinatari utente in qualsiasi ruolo
        /// </summary>
        /// <param name="idUser"></param>
        public Recipient(int idUser)
        {
            this._idUser = idUser;
            this._operationalORinformation = SupportStructures.NotificationType.OPERATIONAL;
        }

        #endregion

        #region public property

        public int ID_ROLE
        {
            get
            {
                return _idRole;
            }

            set
            {
                _idRole = value;
            }
        }

        public int ID_USER
        {
            get
            {
                return _idUser;
            }

            set
            {
                _idUser = value;
            }
        }

        public char OPERATIONAL_OR_INFORMATION
        {
            get
            {
                return _operationalORinformation;
            }

            set
            {
                _operationalORinformation = value;
            }
        }
        #endregion
    }


    /// <summary>
    /// Dato un evento X la classe contiene le informazioni sul Produttore e i destinatari della notifica
    /// </summary>
    public class Actors
    {
        #region private fields

        private Producer _producer;
        private List<Recipient> _recipients;

        #endregion

        #region constructor

        public Actors(Producer producer)
        {
            _recipients = new List<Recipient>();
            _producer = producer;
        }

        #endregion

        #region public property

        public Producer PRODUCER
        {
            get
            {
                return _producer;
            }

            set
            {
                _producer = value;
            }
        }
        public List<Recipient> RECIPIENTS
        {
            get
            {
                return _recipients;
            }

            set
            {
                _recipients = value;
            }
        }

        #endregion

        #region public Method

        /// <summary>
        /// Aggiunge un  destinatario alla lista
        /// </summary>
        /// <param name="receiver"></param>
        public void AddReceiver(Recipient receiver)
        {
            try
            {
                _recipients.Add(receiver);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Aggiunge un destinatario alla lista
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="idUser"></param>
        public void AddRecipient(int idRole, int idUser)
        {
            try
            {
                Recipient receiver = new Recipient(idRole, idUser);
                _recipients.Add(receiver);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Rimuove un destinatario dalla lista
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="idUser"></param>
        public void DeleteRecipient(int idRole, int idUser)
        {
            try
            {
                Recipient delReceiver = new Recipient(idRole, idUser);
                int index = _recipients.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.ID_ROLE == idRole && item.obj.ID_USER == idUser).index;
                _recipients.RemoveAt(index);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Rimuove un destinatario dalla lista
        /// </summary>
        /// <param name="receiver"></param>
        public void DeleteRecipient(Recipient recipient)
        {
            try
            {
                int index = _recipients.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.ID_ROLE == recipient.ID_ROLE && item.obj.ID_USER == recipient.ID_USER).index;
                _recipients.RemoveAt(index);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion
    }
}
